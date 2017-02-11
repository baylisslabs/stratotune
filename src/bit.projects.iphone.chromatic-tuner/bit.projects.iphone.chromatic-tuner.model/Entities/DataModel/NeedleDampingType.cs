using System;
using System.Collections.Generic;
using System.Linq;

namespace bit.projects.iphone.chromatictuner.model
{       
    public class NeedleDampingType : IEntity<NeedleDampingType.Enum>
    {
        public enum Enum
        {
            Low = 0,
            Medium,
            High
        };
        
        
        #region IEntity implementation
        public Enum Id { get; set; }
        public string Description { get; set; }
        #endregion

        public double Alpha { get; set; }
    }
    
    public static class NeedleDampingTypeExtensions
    {
        static NeedleDampingType[] _values;
        static Dictionary<NeedleDampingType.Enum,NeedleDampingType> _lookup;
        
        static NeedleDampingTypeExtensions()
        {
            _values = new [] {
                new NeedleDampingType { Id = NeedleDampingType.Enum.Low, Description = "Low", Alpha = 1.0 }
                ,new NeedleDampingType { Id = NeedleDampingType.Enum.Medium, Description = "Medium",  Alpha = 0.5 }
                ,new NeedleDampingType { Id = NeedleDampingType.Enum.High, Description = "High",  Alpha = 0.25 }
            };
            
            _lookup = _values.ToDictionary((t)=>t.Id);
        }
        
        public static NeedleDampingType[] Values  { get { return _values; } }
        
        public static NeedleDampingType FromId (NeedleDampingType.Enum id)
        {
            if(_lookup.ContainsKey(id)) {
                return _lookup[id];
            }
            throw new ArgumentOutOfRangeException("id",id,"in FromId()");
        }

        public static string ToDbString(this NeedleDampingType obj)
        {
            return Convert.ToString(obj.Id).ToLowerInvariant();
        }
        
        public static NeedleDampingType FromDbString(string str)
        {
            return FromId((NeedleDampingType.Enum)Enum.Parse(typeof(NeedleDampingType.Enum),str,true));
        }
    }
}
