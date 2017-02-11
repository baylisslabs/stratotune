using System;

using bit.shared.audio;
using bit.shared.ios.msgbus;

namespace bit.projects.iphone.chromatictuner.model
{
    public class TunerPitchDataMsg : IMessage
    {      
        public string Id { get { return typeof(TunerPitchDataMsg).FullName; } }
        public int Seq { get; set; }

        public MidiNote? DetectedNote { get; set; }
        public DateTime TimeStamp { get; set; }
        public double A4Calibration { get; set; }
    }

    public class TunerStatusMsg : IMessage
    {
        public string Id { get { return typeof(TunerStatusMsg).FullName; } }
        public int Seq { get; set; }

        public bool MasterEnable { get; set; }
        public TunerMode Mode { get; set; }   
        public bool NoteLock { get; set; }
        public bool AutoPitchPipeSoundBack { get; set; }  
        public double  PitchPipeGain { get; set; }
        public bool IsInterrupted { get; set; }
        public double A4Calibration { get; set; }
        public double Damping { get; set; }
        public MidiNote PitchPipeNote { get; set; }       
        public PitchPipeWaveformType PitchPipeWaveform { get; set; }
        public bool OutputIsBuiltInSpeaker { get; set; }
        public bool AudioInputAvailable { get; set; }       
        //public bool SpeakerOutputOverride { get; set; }
    }

    public class TunerControlChangeMsg : IMessage
    {
        public string Id { get { return typeof(TunerControlChangeMsg).FullName; } }
        public int Seq { get; set; }

        public FlipFlopOp MasterEnable { get; set; }
        public TunerMode? Mode { get; set; }      
        public FlipFlopOp AutoPitchPipeSoundBack { get; set; }
        public FlipFlopOp NoteLock { get; set; }
        //public CounterOp  PitchPipeLevel { get; set; }
    }

    public class TunerSettingsChangeMsg : IMessage
    {
        public string Id { get { return typeof(TunerSettingsChangeMsg).FullName; } }
        public int Seq { get; set; }

        public double? A4Calibration { get; set; }
        public double? Damping { get; set; }
        public MidiNote? PitchPipeNote { get; set; }
        public PitchPipeWaveformType PitchPipeWaveform { get; set; }
    }             
}

