using System;
using System.Collections.Generic;

using MonoTouch.CoreFoundation;
using MonoTouch.Foundation;
using MonoTouch.AudioToolbox;

using bit.shared.ios.audio;
using bit.shared.ios.msgbus;

using bit.shared.audio;
using bit.shared.logging;
using bit.shared.appconfig;

namespace bit.projects.iphone.chromatictuner.model
{
	public class TunerAudioProcessor : IAudioSessionControllerDelegate
	{
        static Logger _log = LogManager.GetLogger("TunerAudioProcessor");    

        public readonly MidiNote MAX_NOTE = MidiNoteExtensions.Notes.C10;
        public readonly MidiNote MIN_NOTE = MidiNoteExtensions.Notes.C0;
               
        private const double PITCH_PIPE_PAN_UPDATE_SECS = 0.1;

        // Audio-output dispatch msg ids
        private enum AudioOutDispatchMsgId
        {
            EM_UPDATE_WAVEFORM_MSG = 1,       
            EM_UPDATE_FREQ_MSG
        }
       
        private IMsgBus _msgBus;
        private IChannel<TunerControlChangeMsg> _tunerControlChangeMsgChannel;
        private IChannel<TunerSettingsChangeMsg> _tunerSettingsMsgChannel;
        private IChannel<TunerPitchDataMsg> _tunerPitchDataMsgChannel;
        private IChannel<TunerStatusMsg> _tunerStatusMsgChannel;
      
        private AudioSessionController _asc;
		private AudioInputStream _ais;
        private AudioOutputStream _aos;
		private PitchDetector _pd;
        private BackgroundAudioProcessor _bap;
        private PitchDetectorParams _pdp;
        private TunerFilterContext _tfc;
        private EnvelopeModulator _em;
        private readonly Dictionary<PitchPipeWaveformType.Enum,AdjustablePeriodicEnvelopeFunc> _waveForms;
               
        private TunerStatusMsg _tunerStatus;
        private bool _initialised;       

        public TunerAudioProcessor (IMsgBus msgBus, TunerConfigSection config)
        {                
            _msgBus = msgBus;
            _tunerControlChangeMsgChannel = _msgBus.CreateChannel<TunerControlChangeMsg>(new ChannelOptions { QueueType = ChannelOptions.QueueTypeOptions.Queue });
            _tunerPitchDataMsgChannel = _msgBus.CreateChannel<TunerPitchDataMsg>(new ChannelOptions { QueueType = ChannelOptions.QueueTypeOptions.Store });
            _tunerSettingsMsgChannel = _msgBus.CreateChannel<TunerSettingsChangeMsg>(new ChannelOptions { QueueType = ChannelOptions.QueueTypeOptions.Queue });           
            _tunerStatusMsgChannel = _msgBus.CreateChannel<TunerStatusMsg>(new ChannelOptions { QueueType = ChannelOptions.QueueTypeOptions.Store });   
            _tunerControlChangeMsgChannel.Subscribe(handleControlChangeMsg);
            _tunerSettingsMsgChannel.Subscribe(handleSettingsChangeMsg);       

            _tunerStatus = new TunerStatusMsg();

            _waveForms = new Dictionary<PitchPipeWaveformType.Enum, AdjustablePeriodicEnvelopeFunc>();
            _waveForms[PitchPipeWaveformType.Enum.Sine] = new EnvelopeFuncExtensions.SineWave();
            _waveForms[PitchPipeWaveformType.Enum.Sawtooth] = new EnvelopeFuncExtensions.SawtoothWave();
            _waveForms[PitchPipeWaveformType.Enum.Square] = new EnvelopeFuncExtensions.SquareWave();
            _waveForms[PitchPipeWaveformType.Enum.Triangular] = new EnvelopeFuncExtensions.TriangularWave();

            _asc = new AudioSessionController(this);
            _initialised = _asc.Initialise(()=>{
                _tfc = new TunerFilterContext(this.MIN_NOTE,this.MAX_NOTE);
                _pdp = new PitchDetectorParams();
                _pd = new PitchDetector(_pdp, this.processPitchResultUnsafeThread);
                _bap = new BackgroundAudioProcessor(_pd);
                _em = new EnvelopeModulator() {  };
                _ais = new AudioInputStream(config.NumInputPackets,config.NumInputBuffers,config.SamplingRate,_bap);   
                _aos = new AudioOutputStream(config.NumOutputPackets,config.NumOutputBuffers,config.SamplingRate,_em);  
                               
                _aos.MasterGain = (float)config.PitchPipeGain;
                _tunerStatus.A4Calibration = _tfc.A4Calibration;
                _tunerStatus.Damping = _tfc.Damping;
                _tunerStatus.PitchPipeNote = MidiNoteExtensions.Notes.C4;
                _tunerStatus.PitchPipeWaveform =  PitchPipeWaveformTypeExtensions.FromId(PitchPipeWaveformType.Enum.Sine);
                _tunerStatus.PitchPipeGain = _aos.MasterGain;
                _tunerStatus.AudioInputAvailable = _asc.IsInputAvailable;
                setPitchPipeWaveform(_tunerStatus.PitchPipeWaveform);
                setPitchPipeNote(_tunerStatus.PitchPipeNote);
                return true;
            });                       
		}
               
        private void handleControlChangeMsg (TunerControlChangeMsg msg)
        {
            if (_initialised)
            {       
                var newMasterEnable = msg.MasterEnable.NextState (_tunerStatus.MasterEnable);
                var newMode = msg.Mode.HasValue?msg.Mode.Value:_tunerStatus.Mode;    
                              
                var newAutoPitchPipeSoundBack = 
                    newMasterEnable 
                    && !_tunerStatus.OutputIsBuiltInSpeaker
                    && newMode==TunerMode.AutoPitchPipe
                    && msg.AutoPitchPipeSoundBack.NextState (_tunerStatus.AutoPitchPipeSoundBack);   
                var newNoteLock = msg.NoteLock.NextState(_tunerStatus.NoteLock);
                                                                              
                _ais.SetEnable (newMasterEnable && newMode != TunerMode.PitchPipe);
                _aos.SetEnable(newMasterEnable && (newMode == TunerMode.PitchPipe || newAutoPitchPipeSoundBack));
                                             
                if (newMasterEnable && (!_asc.IsPlayBackOn || !_asc.IsRecordingOn)) {
                    start();
                }

                if(!(_tunerStatus.MasterEnable&&_tunerStatus.Mode==TunerMode.PitchPipe&&_tunerStatus.NoteLock) &&
                   (newMasterEnable&&newMode==TunerMode.PitchPipe&&newNoteLock)) {
                    _tunerStatus.PitchPipeNote = _tunerStatus.PitchPipeNote.NearestNote();
                }
                                                           
                _tunerStatus.MasterEnable = newMasterEnable;
                _tunerStatus.Mode = newMode;           
                _tunerStatus.AutoPitchPipeSoundBack = newAutoPitchPipeSoundBack;  
                _tunerStatus.NoteLock = newNoteLock;
            }
            _tunerStatusMsgChannel.Publish(_tunerStatus);  
        }

        private void handleSettingsChangeMsg (TunerSettingsChangeMsg msg)
        {
            if (_initialised) {           
                if (msg.A4Calibration.HasValue) {
                    _tfc.SetA4Calibration (msg.A4Calibration.Value);                         
                    if (!msg.PitchPipeNote.HasValue) {
                        setPitchPipeNote (_tunerStatus.PitchPipeNote);
                    }
                }
                if (msg.Damping.HasValue) {
                    _tfc.SetDamping (msg.Damping.Value);
                }   
                if (msg.PitchPipeNote.HasValue) {
                    MidiNote newNote = msg.PitchPipeNote.Value;
                    if(msg.PitchPipeNote.Value < this.MIN_NOTE) {
                        newNote = this.MIN_NOTE;
                    } else if (msg.PitchPipeNote.Value > this.MAX_NOTE) {
                        newNote = this.MAX_NOTE;
                    }
                    setPitchPipeNote (newNote);
                    _tunerStatus.PitchPipeNote = newNote;
                }                 
                if (msg.PitchPipeWaveform != null) {
                    setPitchPipeWaveform (msg.PitchPipeWaveform);
                    _tunerStatus.PitchPipeWaveform = msg.PitchPipeWaveform;
                }

                _tunerStatus.A4Calibration = _tfc.A4Calibration;
                _tunerStatus.Damping = _tfc.Damping;
            }
            _tunerStatusMsgChannel.Publish(_tunerStatus);
        }
               
        // todo: message bus these     
        private void processPitchResultUnsafeThread (int seq, PitchDetectorResult pdr)
        {
            DispatchQueue.MainQueue.DispatchAsync (() => {
                this.processPitchResult (seq, pdr);});
        }
        
        private void processPitchResult (int seq, PitchDetectorResult pdr)
        {
            var pdm = _tfc.ProcessNext(seq,pdr);
            _tunerPitchDataMsgChannel.Publish(pdm);                      
        }  
        
        private bool start ()
        {                
            bool success = _asc.StartRecording (canDefer: true);
            success = _asc.StartPlayback(canDefer:true) && success;
            return success;
        }
        
        public void stop()
        {                      
            _asc.StopRecording();     
            _asc.StopPlayback();
        }
                           
        #region IAudioSessionDelegate implementation

        // todo: message bus these  
        void IAudioSessionControllerDelegate.StartingRecording()
        {           
            _tfc.Start ();
            _ais.Start (); 
        }

        void IAudioSessionControllerDelegate.StoppingRecording()
        {
            _ais.Stop(false);
            _tfc.Stop ();           
        }

        void IAudioSessionControllerDelegate.InterruptingRecording ()
        {
            _tunerStatus.IsInterrupted = true;
            _tunerStatusMsgChannel.Publish(_tunerStatus);
        }

        void IAudioSessionControllerDelegate.ResumingRecording ()
        {
            _tfc.Start ();
            _ais.Start ();          
            _tunerStatus.IsInterrupted = false;
            _tunerStatusMsgChannel.Publish(_tunerStatus);
        }

        void IAudioSessionControllerDelegate.StartingPlayback()
        {                      
            _aos.Start ();
        }
        
        void IAudioSessionControllerDelegate.StoppingPlayback()
        {          
            _aos.Stop(true);
        }
        
        void IAudioSessionControllerDelegate.InterruptingPlayback ()
        {
            _tunerStatus.IsInterrupted = true;
            _tunerStatusMsgChannel.Publish(_tunerStatus);
        }
        
        void IAudioSessionControllerDelegate.ResumingPlayback ()
        {
            _aos.Start ();
            _tunerStatus.IsInterrupted = false;
            _tunerStatusMsgChannel.Publish(_tunerStatus);
        }

        void IAudioSessionControllerDelegate.HardwareRouteChange (
            AudioSessionInputRouteKind input,
            AudioSessionOutputRouteKind[] outputs)
        {
            _tunerStatus.OutputIsBuiltInSpeaker = 
                outputs != null && 
                    (Array.FindIndex(outputs,o=>o==AudioSessionOutputRouteKind.BuiltInSpeaker)) >= 0;   

            _tunerStatusMsgChannel.Publish(_tunerStatus);
        }

        void IAudioSessionControllerDelegate.InputAvailable (bool isAvailable)
        {
            _tunerStatus.AudioInputAvailable = isAvailable;            
            _tunerStatusMsgChannel.Publish(_tunerStatus);
        }

        #endregion
                     
        //
        // todo: message bus these      
        // could be done with a push/pull msg bus queue-type
        // or a "manual" channel deliver, triggered by a callback from the AudioOuputQ callback
        //
        private void setPitchPipeNote (MidiNote note)
        {
            double hz = _tfc.NoteFrequency (note);
        
            _aos.DispatchAsync ((int)AudioOutDispatchMsgId.EM_UPDATE_FREQ_MSG, true, () => {
                _log.Trace ("Updating pitchpipe freq to: {0}", hz);
                // TODO: only set current wf             
                foreach (var wf in _waveForms) {
                    wf.Value.AdjustFreq(hz,_aos.CurrentSliceTime);
                }
            });
        }

        public void setPitchPipeWaveform (PitchPipeWaveformType wt)
        {                      
            _aos.DispatchAsync((int)AudioOutDispatchMsgId.EM_UPDATE_WAVEFORM_MSG,true,()=>{
                _log.Trace("Updating pitchpipe waveform to: {0}", wt.Id);
                if(_waveForms.ContainsKey(wt.Id)) {
                    _em.Envelope = _waveForms[wt.Id];
                    _em.Gain = wt.Gain;
                }               
            });
        }                  
	}
}

