using System;
using System.Collections.Generic;

namespace bit.projects.iphone.chromatictuner.model
{                 
    public class UserSettings : IEntity<int>
    {       
        public int Id { get; set; }
        public string Name { get; set; }
        public bool   DisallowDelete { get; internal set; } 
        public double A4Calibration { get; set; }
        public MusicNotationType Notation { get; set; }
        public TranspositionType Transposition { get; set; }
        public TemperamentType Temperament { get; set; }
        public NeedleDampingType NeedleDamping { get; set; }
        public bool FrequencyDisplay { get; set; }
        public PitchPipeWaveformType PitchPipeWaveform;               
    }

    public static class UserSettingsExtensions
    {
        public static readonly double A4CalibrationMax = 880;
        public static readonly double A4CalibrationMin = 220;

        public static UserSettings CreateWithName(string name)
        {
            return new UserSettings {
                Id = 0,
                Name = name,
                DisallowDelete = false,
                A4Calibration = 440d,
                Notation = MusicNotationTypeExtensions.FromId(MusicNotationType.Enum.English),
                Transposition =  TranspositionTypeExtensions.FromId(TranspositionType.Enum.C),
                Temperament = TemperamentTypeExtensions.FromId(TemperamentType.Enum.EqualTemperament),
                NeedleDamping = NeedleDampingTypeExtensions.FromId(NeedleDampingType.Enum.Low),
                FrequencyDisplay = false,
                PitchPipeWaveform = PitchPipeWaveformTypeExtensions.FromId(PitchPipeWaveformType.Enum.Sine)
            };
        }
    }
     
}

