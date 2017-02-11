using System;
using System.Collections.Generic;
using System.Linq;

namespace bit.projects.iphone.chromatictuner.model
{          
    public class PitchPipeWaveformType : IEntity<PitchPipeWaveformType.Enum>
    {
        public enum Enum
        {
            Sine = 0,
            Square,
            Sawtooth,
            Triangular
        };
        
        
        #region IEntity implementation
        public Enum Id { get; set; }
        public string Description { get; set; }
        #endregion

        public double Gain { get; set; }
    }
    
    public static class PitchPipeWaveformTypeExtensions
    {
        static PitchPipeWaveformType[] _values;
        static Dictionary<PitchPipeWaveformType.Enum,PitchPipeWaveformType> _lookup;
        
        static PitchPipeWaveformTypeExtensions()
        {
            _values = new [] {
                new PitchPipeWaveformType { Id = PitchPipeWaveformType.Enum.Sine, Description = "Sine", Gain = 1.0 }
                ,new PitchPipeWaveformType { Id = PitchPipeWaveformType.Enum.Square, Description = "Square", Gain = 0.25 }
                ,new PitchPipeWaveformType { Id = PitchPipeWaveformType.Enum.Sawtooth, Description = "Sawtooth", Gain = 0.25 }
                ,new PitchPipeWaveformType { Id = PitchPipeWaveformType.Enum.Triangular, Description = "Triangular", Gain = 1.0 }
            };
            
            _lookup = _values.ToDictionary((t)=>t.Id);
        }
        
        public static PitchPipeWaveformType[] Values  { get { return _values; } }
        
        public static PitchPipeWaveformType FromId (PitchPipeWaveformType.Enum id)
        {
            if(_lookup.ContainsKey(id)) {
                return _lookup[id];
            }
            throw new ArgumentOutOfRangeException("id",id,"in FromId()");
        }

        public static string ToDbString(this PitchPipeWaveformType obj)
        {
            return Convert.ToString(obj.Id).ToLowerInvariant();
        }

        public static PitchPipeWaveformType FromDbString(string str)
        {
            return FromId((PitchPipeWaveformType.Enum)Enum.Parse(typeof(PitchPipeWaveformType.Enum),str,true));
        }
    }
}
