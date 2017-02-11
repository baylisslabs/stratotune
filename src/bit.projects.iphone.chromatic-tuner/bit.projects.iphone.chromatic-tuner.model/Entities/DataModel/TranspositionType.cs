using System;
using System.Collections.Generic;
using System.Linq;

namespace bit.projects.iphone.chromatictuner.model
{       
    public class TranspositionType : IEntity<TranspositionType.Enum>
    {
        public enum Enum
        {
            G,
            A_,
            A,
            B_,
            B,
            C,
            D_,
            D,
            E_,
            E,
            F,
            G_
        };
                
        #region IEntity implementation
        public Enum Id { get; set; }     
        public double SemiToneShift { get; set; }
        public string Description { get; set; }
        #endregion
    }
    
    public static class TranspositionTypeExtensions
    {
        static TranspositionType[] _values;
        static Dictionary<TranspositionType.Enum,TranspositionType> _lookup;
        
        static TranspositionTypeExtensions()
        {
            _values = new [] {
                new TranspositionType { Id = TranspositionType.Enum.G,   SemiToneShift = -5d, Description = "G" }              
                ,new TranspositionType { Id = TranspositionType.Enum.A_, SemiToneShift = -4d, Description = "A\u266d" }
                ,new TranspositionType { Id = TranspositionType.Enum.A,  SemiToneShift = -3d, Description = "A" }
                ,new TranspositionType { Id = TranspositionType.Enum.B_, SemiToneShift = -2d, Description = "B\u266d" }
                ,new TranspositionType { Id = TranspositionType.Enum.B,  SemiToneShift = -1d, Description = "B" }
                ,new TranspositionType { Id = TranspositionType.Enum.C,  SemiToneShift = 0,   Description = "C" }
                ,new TranspositionType { Id = TranspositionType.Enum.D_, SemiToneShift = 1d,  Description = "C\u266f" }
                ,new TranspositionType { Id = TranspositionType.Enum.D,  SemiToneShift = 2d,  Description = "D" }
                ,new TranspositionType { Id = TranspositionType.Enum.E_, SemiToneShift = 3d,  Description = "E\u266d" }
                ,new TranspositionType { Id = TranspositionType.Enum.E,  SemiToneShift = 4d,  Description = "E" }
                ,new TranspositionType { Id = TranspositionType.Enum.F,  SemiToneShift = 5d,  Description = "F" }
                ,new TranspositionType { Id = TranspositionType.Enum.G_, SemiToneShift = 6d,  Description = "F\u266f" }
            };
            
            _lookup = _values.ToDictionary((t)=>t.Id);
        }
        
        public static TranspositionType[] Values  { get { return _values; } }
        
        public static TranspositionType FromId (TranspositionType.Enum id)
        {
            if(_lookup.ContainsKey(id)) {
                return _lookup[id];
            }
            throw new ArgumentOutOfRangeException("id",id,"in FromId()");
        }

        public static string ToDbString(this TranspositionType obj)
        {
            return Convert.ToString(obj.Id).ToLowerInvariant();
        }
        
        public static TranspositionType FromDbString(string str)
        {
            return FromId((TranspositionType.Enum)Enum.Parse(typeof(TranspositionType.Enum),str,true));
        }
    }
}
