using System;

using bit.shared.appconfig;

namespace bit.projects.iphone.chromatictuner.model
{
    public class TunerConfigSection : ConfigSection
    {
        public override string Name { get { return "bit.projects.iphone.chromatictuner.TunerConfigSection"; } }

        public int NumInputPackets { get; set; }
        public int NumInputBuffers { get; set; }
        public int NumOutputPackets { get; set; }
        public int NumOutputBuffers { get; set; }
        public double SamplingRate { get; set; }
        public double PitchPipeGain { get; set; }
    }
}

