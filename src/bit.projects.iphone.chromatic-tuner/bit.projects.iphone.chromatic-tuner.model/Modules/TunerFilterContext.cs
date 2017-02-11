using System;
using System.Collections.Generic;

using bit.shared.audio;
using bit.shared.logging;
using bit.shared.numerics;

namespace bit.projects.iphone.chromatictuner.model
{
    public class TunerFilterContext
    {
        static Logger _log = LogManager.GetLogger("TunerFilterContext");

        private enum State
        {
            Off,
            Idle,
            Tracking
        }

        private delegate MidiNote? filterAction(MidiNote? detectedNote);

        private State _state;
        private MidiNote? _smoothedNote;
        private double _a4Hz;
        private double _alpha;

        private readonly double _lockThreshold;
        private readonly double _octaveJumpThreshold;
        private readonly bool _discardFirst;
        private readonly Dictionary<State,filterAction> _actions;
        private readonly MidiNote _minNote;
        private readonly MidiNote _maxNote;

        public double A4Calibration { get { return _a4Hz; } }
        public double Damping { get { return _alpha; } }

        public TunerFilterContext (MidiNote minNote, MidiNote maxNote)
        {
            _state = State.Off;
            _actions = new Dictionary<State, filterAction>();
            _actions.Add(State.Off,this.processOff);
            _actions.Add(State.Idle,this.processIdle);
            _actions.Add(State.Tracking,this.processTracking);
            //_actions.Add(State.LockedOn,this.processLockedOn);

            // settings
            _minNote = minNote;
            _maxNote = maxNote;
            _discardFirst = true;
            _alpha = 1;
            _lockThreshold = 0.025; // +/-2.5 cents
            _octaveJumpThreshold = 1.5; // +/-150 cents
            _a4Hz = MidiNote.A4_STANDARD_HZ;
        }

        public void SetA4Calibration (double hz)
        {
            _log.Debug("SetA4Calibration({0})",hz);
            if (hz > 0) {
                _a4Hz = hz;
            }
        }

        public void SetDamping(double value)
        {
            _log.Debug("SetDamping({0})",value);
            if (value >= 1) {
                _alpha = value;
            }
        }

        public double NoteFrequency (MidiNote note)
        {
            return note.ToHz(_a4Hz);
        }

        public TunerPitchDataMsg ProcessNext (int seq, PitchDetectorResult rawData)
        {
            MidiNote? detectedNote = getNote (rawData);

            if (detectedNote != null) {
                _log.Debug (() => "[" + seq + "] F0_Hz = " + rawData.F_0_Hz.GetValueOrDefault ()
                    + " T(proc) = " + rawData.ProcessingTimeMs + " Note = "
                    + detectedNote.Value.SemiToneWithFraction ());                                               

                if(detectedNote.Value < _minNote || detectedNote.Value > _maxNote) {
                    detectedNote = null;
                }
            }                       
                       
            if (_actions.ContainsKey (_state)) {
                detectedNote = _actions [_state] (detectedNote);
            }

            return new TunerPitchDataMsg { DetectedNote = detectedNote, TimeStamp = rawData.TimeStamp, A4Calibration = _a4Hz };
        }

        public void Stop()
        {
            _state = State.Off;
        }

        public void Start ()
        {
            _state = State.Idle;
        }

        private MidiNote? getNote(PitchDetectorResult rawData)
        {
            MidiNote? detectedNote = null;
            if(rawData.F_0_Hz.HasValue && rawData.F_0_Hz.Value > 0) {              
                detectedNote = MidiNote.FromHz(rawData.F_0_Hz.Value,_a4Hz);                                                       
            }
            return detectedNote;
        }

        private MidiNote? processOff(MidiNote? detected)
        {
            return null;
        }

        private MidiNote? processIdle (MidiNote? detected)
        {
            if (detected != null) {
                _state = State.Tracking;
                if(_discardFirst) {
                    detected = null;
                }
                _smoothedNote = detected;
            }
            return detected;
        }

        private MidiNote? processTracking (MidiNote? detected)
        {
            if (detected == null) {
                _state = State.Idle;
            } else {
                if(_smoothedNote==null) {
                    _smoothedNote = detected;
                }
                else {
                    var dist = detected.Value.SemiToneDistance(_smoothedNote.Value);
                    var absDist = Math.Abs(dist);

                    /* filter: 2nd harmonic jump up tendency */
                    if(absDist <= _octaveJumpThreshold) {
                        detected = _smoothedNote.Value.Transpose(dist);
                    }
                   
                    if(absDist <= _lockThreshold) {
                        detected = applySmoothing(_smoothedNote.Value,dist);                    
                    }
                    else {
                        _smoothedNote = detected;
                    }
                }
            }
            return detected;
        }
               
        private MidiNote? applySmoothing (MidiNote last, double dist)
        {           
         
            var st_last = last.SemiToneWithFraction ();
            var st_curr = st_last + dist;
            var st_next_ema = _alpha*(st_curr - st_last) + st_last;

            return MidiNote.FromOctaveAndSemitone(last.Octave(),st_next_ema);
        }
    }
}

