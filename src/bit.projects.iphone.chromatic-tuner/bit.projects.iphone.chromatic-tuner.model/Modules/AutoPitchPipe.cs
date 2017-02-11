using System;

using bit.shared.ios.msgbus;

using bit.shared.audio;
using bit.shared.logging;
using bit.shared.appconfig;

namespace bit.projects.iphone.chromatictuner.model
{
    public class AutoPitchPipe
    {
        private readonly int SAME_NOTE_HOLD = 2;     

        private IMsgBus _msgBus;
        private TunerStatusMsg _tunerStatus;
        private int _noteSameCount;
        private bool _noteHold;
        private MidiNote _soundBackNote;
        private MidiNote _lastNote;

        //private IChannel<TunerControlChangeMsg> _tunerControlChangeMsgChannel;
        //private IChannel<TunerSettingsChangeMsg> _tunerSettingsMsgChannel;
        //private IChannel<TunerPitchDataMsg> _tunerPitchDataMsgChannel;
        //private IChannel<TunerStatusMsg> _tunerStatusMsgChannel;

        public AutoPitchPipe (IMsgBus msgBus)
        {
            _msgBus = msgBus;
            _msgBus.Subscribe<TunerPitchDataMsg>(processTunerPitchDataMsg);
            _msgBus.Subscribe<TunerStatusMsg>(processTunerStatusMsg);
        }

        private void processTunerPitchDataMsg (TunerPitchDataMsg msg)
        {                      
            if (_tunerStatus.Mode==TunerMode.AutoPitchPipe) {
                if(msg.DetectedNote!=null) {
                    int semiTone = msg.DetectedNote.Value.SemiTone();
                    var autoNote = MidiNote.FromOctaveAndSemitone(semiTone<(int)MidiNoteExtensions.SemiTones.Es?5:4,semiTone);        

                    if(autoNote==_lastNote) {
                        _noteSameCount++;
                    } else {
                        _noteSameCount--;
                    }
                                                                    
                    _lastNote = autoNote;
                }
                else {                   
                    _noteSameCount--;
                }

                if(_noteSameCount>=SAME_NOTE_HOLD) {
                    _noteSameCount = SAME_NOTE_HOLD;
                    _noteHold = true;
                }
                else if(_noteSameCount<=0) {
                    _noteSameCount = 0;
                    _noteHold = false;
                }
                
                if(!_noteHold) {
                    _soundBackNote = _lastNote;
                }                     

                if(_noteSameCount>0) {
                    if(!_tunerStatus.NoteLock) {
                        _msgBus.Publish(new TunerSettingsChangeMsg { PitchPipeNote = _soundBackNote });
                    }
                    _msgBus.Publish(new TunerControlChangeMsg { AutoPitchPipeSoundBack = FlipFlopOp.Set });   
                }
                else {
                    _msgBus.Publish(new TunerControlChangeMsg { AutoPitchPipeSoundBack = FlipFlopOp.Reset });
                }
            }
        }     
        
        private void processTunerStatusMsg (TunerStatusMsg msg)
        {                      
            _tunerStatus = msg;
            if (msg.Mode != TunerMode.AutoPitchPipe) {
                _noteSameCount = 0;
                _noteHold = false;
                _lastNote = _soundBackNote = new MidiNote();
            }
        }   
    }
}

