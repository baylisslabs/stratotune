using System;
#if DEBUG
using System.Linq; // debug
#endif

using MonoTouch.CoreFoundation;
using MonoTouch.Foundation;
using MonoTouch.AudioToolbox;

using bit.shared.logging;

namespace bit.shared.ios.audio
{
	public class AudioSessionController
	{
        static Logger _log = LogManager.GetLogger("AudioSessionController");
                               
        private bool _initialised;
        private AudioSessionControllerState _processingState;
        private IAudioSessionControllerDelegate _asd;

        public bool IsPlayBackOn { get { return _processingState.IsPlayBackOn; } }
        public bool IsRecordingOn { get { return _processingState.IsRecordingOn; } }
        public bool IsInputAvailable { get { return AudioSession.AudioInputAvailable; } }

        public AudioSessionController (IAudioSessionControllerDelegate asd)
        {
            _processingState = new AudioSessionControllerState {
                Recording = AudioSessionControllerState.Status.Off,
                Playback = AudioSessionControllerState.Status.Off };

            _asd = asd;
		}

        public bool Initialise (Func<bool> customInitialisation)
        {
            _log.Debug ("Initialise()");

            if (_initialised) {
                return true;
            }

            try {
                if (initialiseAudioSession()) {
                    _initialised = customInitialisation();                   
                }
            }
            catch (Exception ex) {
                _log.Error("Initialise() failed",ex);
            }

            return _initialised;
        }


        public bool StartRecording (bool canDefer)
        {           
            if (!_initialised) {
                return false;
            }

            _log.Debug ("StartRecording()");
                       
            try {
                if(_processingState.IsInterrupted()) {
                    if(canDefer && _processingState.Recording == AudioSessionControllerState.Status.Off) {
                        _processingState.Recording = AudioSessionControllerState.Status.Pending;
                        return true;
                    }
                    return false;
                }

               if(!_processingState.IsActive()) {
                    AudioSession.SetActive (true);
                }

                if(_processingState.Recording == AudioSessionControllerState.Status.Off) {
                    _processingState.Recording = AudioSessionControllerState.Status.On;
                    //setCategory(_processingState.ToCategory());   
                    _asd.StartingRecording();                   
                    return true;                   
                }               
            } catch (Exception ex) {
                _log.Error ("Start() failed", ex);
            }     
            return false;
		}
		
        public void StopRecording ()
        {          
            if (!_initialised) {
                return;
            }
            
            _log.Debug ("StopRecording()");
                        
            try {                              
                if(_processingState.Recording != AudioSessionControllerState.Status.Off) {
                    _processingState.Recording = AudioSessionControllerState.Status.Off;
                    //setCategory(_processingState.ToCategory());   
                    _asd.StoppingRecording();                                                       
                }
            } catch (Exception ex) {
                _log.Error ("StopRecording() failed", ex);
            }                
		}

        public bool StartPlayback (bool canDefer)
        {           
            if (!_initialised) {
                return false;
            }
            
            _log.Debug ("StartPlayback()");
            
            
            try {
                if(_processingState.IsInterrupted()) {
                    if(canDefer && _processingState.Playback == AudioSessionControllerState.Status.Off) {
                        _processingState.Playback = AudioSessionControllerState.Status.Pending;
                        return true;
                    }
                    return false;
                }

                if(!_processingState.IsActive()) {
                    AudioSession.SetActive (true);
                }
                
                if(_processingState.Playback == AudioSessionControllerState.Status.Off) {
                    _processingState.Playback = AudioSessionControllerState.Status.On;
                    //setCategory(_processingState.ToCategory());
                    _asd.StartingPlayback();                   
                    return true;                   
                }
            } catch (Exception ex) {
                _log.Error ("StartPlayback() failed", ex);
            }     
            return false;
        }
        
        public void StopPlayback ()
        {          
            if (!_initialised) {
                return;
            }
            
            _log.Debug ("StopPlayback()");
            
            try {                              
                if(_processingState.Playback != AudioSessionControllerState.Status.Off) {
                    _processingState.Playback = AudioSessionControllerState.Status.Off;
                    //setCategory(_processingState.ToCategory());
                    _asd.StoppingPlayback();                                                       
                }
            } catch (Exception ex) {
                _log.Error ("StopPlayback() failed", ex);
            }                
        }
                      
        private void interrupt ()
        {
            if (!_initialised) {
                return;
            }
            
            try {                              
                if(_processingState.Recording == AudioSessionControllerState.Status.On) {
                    _processingState.Recording = AudioSessionControllerState.Status.Interrupted;                   
                    _asd.InterruptingRecording();                                                       
                }
            } catch (Exception ex) {
                _log.Error ("interruptRecording() failed", ex);
            }  

            try {                              
                if(_processingState.Playback == AudioSessionControllerState.Status.On) {
                    _processingState.Playback = AudioSessionControllerState.Status.Interrupted;                   
                    _asd.InterruptingPlayback();                                                       
                }
            } catch (Exception ex) {
                _log.Error ("interruptPlayback() failed", ex);
            }  
        }

        private void resume ()
        {
            if (!_initialised) {
                return;
            }

            try {
                if(!_processingState.IsActive()&&(_processingState.IsInterrupted()||_processingState.AnyPending())) {
                    AudioSession.SetActive (true);
                }

                if(_processingState.Recording == AudioSessionControllerState.Status.Interrupted) {
                    _processingState.Recording = AudioSessionControllerState.Status.On;                   
                    _asd.ResumingRecording();                                                       
                }

                if(_processingState.Playback == AudioSessionControllerState.Status.Interrupted) {
                    _processingState.Playback = AudioSessionControllerState.Status.On;                   
                    _asd.ResumingPlayback();                                                       
                }

                if(_processingState.Recording == AudioSessionControllerState.Status.Pending) {
                    _processingState.Recording = AudioSessionControllerState.Status.On;                   
                    _asd.StartingRecording();                                                       
                }
                
                if(_processingState.Playback == AudioSessionControllerState.Status.Pending) {
                    _processingState.Playback = AudioSessionControllerState.Status.On;                   
                    _asd.StartingPlayback();                                                       
                }
            } catch (Exception ex) {
                _log.Error ("resume() failed", ex);
            }                       
        }
               
        private bool initialiseAudioSession ()
        {
            try {
                AudioSession.Initialize ();
                setCategory(AudioSessionCategory.PlayAndRecord);   
                AudioSession.Mode = AudioSessionMode.Default;
                AudioSession.Interrupted += audioSessionInterrupted;
                AudioSession.Resumed += audioSessionResumed;    
                AudioSession.AddListener(AudioSessionProperty.AudioRouteChange,audioRouteChange);
                AudioSession.AddListener(AudioSessionProperty.AudioInputAvailable,audioInputAvailable);

                return true;
            } 
            catch (Exception ase) {
                _log.Error("initialiseAudioSession() failed",ase);
                return false;
            }
        }

        private void audioSessionInterrupted (object sender, EventArgs e)
        {
            _log.Debug ("audioSessionInterrupted()");

            if (!_initialised) {
                return;
            }

            interrupt();
        }

        private void audioSessionResumed (object sender, EventArgs e)
        {
            _log.Debug ("audioSessionResumed()");

            if (!_initialised) {
                return;
            }

            resume();
        }   

        private void audioRouteChange (AudioSessionProperty property, int size, IntPtr data)
        {
#if DEBUG
            _log.Debug(()=>(string.Format("audioRouteChange({0},{1})",
                       AudioSession.InputRoute,
                       string.Join(",",AudioSession.OutputRoutes.Select(kind=>kind.ToString())))));                     
#endif
            _asd.HardwareRouteChange(AudioSession.InputRoute, AudioSession.OutputRoutes);
        }

        private void audioInputAvailable (AudioSessionProperty property, int size, IntPtr data)
        {
#if DEBUG
            _log.Debug(()=>(string.Format("audioInputAvailable({0})", this.IsInputAvailable)));
#endif
            _asd.InputAvailable(AudioSession.AudioInputAvailable);
        }

        private void setCategory (AudioSessionCategory category)
        {
            if (AudioSession.Category != category) {
                AudioSession.Category = category;
                if (category == AudioSessionCategory.PlayAndRecord) {
                    AudioSession.OverrideCategoryDefaultToSpeaker = true;
                }
            }
        }
	}
}

