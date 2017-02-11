using System;
using System.Collections.Generic;
using System.Linq;

namespace bit.projects.iphone.chromatictuner.model
{       
    public class TemperamentType : IEntity<TemperamentType.Enum>
    {
        public enum Enum
        {
            EqualTemperament = 0
        };
        
        
        #region IEntity implementation
        public Enum Id { get; set; }
        public string Description { get; set; }
        #endregion
    }
    
    public static class TemperamentTypeExtensions
    {
        static TemperamentType[] _values;
        static Dictionary<TemperamentType.Enum,TemperamentType> _lookup;
        
        static TemperamentTypeExtensions()
        {
            _values = new [] {
                new TemperamentType { Id = TemperamentType.Enum.EqualTemperament, Description = "Equal Temperament" }               
            };
            
            _lookup = _values.ToDictionary((t)=>t.Id);
        }
        
        public static TemperamentType[] Values  { get { return _values; } }
        
        public static TemperamentType FromId (TemperamentType.Enum id)
        {
            if(_lookup.ContainsKey(id)) {
                return _lookup[id];
            }
            throw new ArgumentOutOfRangeException("id",id,"in FromId()");
        }

        public static string ToDbString(this TemperamentType obj)
        {
            return Convert.ToString(obj.Id).ToLowerInvariant();
        }
        
        public static TemperamentType FromDbString(string str)
        {
            return FromId((TemperamentType.Enum)Enum.Parse(typeof(TemperamentType.Enum),str,true));
        }
    }
}
