using System;
using System.Collections.Generic;
using System.Linq;

namespace bit.projects.iphone.chromatictuner.model
{          
    public class MusicNotationType : IEntity<MusicNotationType.Enum>
    {
        public enum Enum
        {
            English = 0,
            English_Sharp,
            English_Flat,
            Solfege_FixedDo,
            NorthernEuropean
        };


        #region IEntity implementation
        public Enum Id { get; set; }
        public string Description { get; set; }
        #endregion

        public string[] NoteLookup { get; set; }
    }

    public static class MusicNotationTypeExtensions
    {
        static MusicNotationType[] _values;
        static Dictionary<MusicNotationType.Enum,MusicNotationType> _lookup;

        static MusicNotationTypeExtensions()
        {
            _values = new [] {
                new MusicNotationType { Id = MusicNotationType.Enum.English, Description = "English", NoteLookup = new string[] {"C","C\u266f","D","E\u266d","E","F","F\u266f","G","A\u266d","A","B\u266d","B"} }
                ,new MusicNotationType { Id = MusicNotationType.Enum.English_Sharp, Description = "English \u266f", NoteLookup = new string[] {"C","C\u266f","D","D\u266f","E","F","F\u266f","G","G\u266f","A","A\u266f","B"} }
                ,new MusicNotationType { Id = MusicNotationType.Enum.English_Flat, Description = "English \u266d", NoteLookup = new string[] {"C","D\u266d","D","E\u266d","E","F","G\u266d","G","A\u266d","A","B\u266d","B"} }
                ,new MusicNotationType { Id = MusicNotationType.Enum.Solfege_FixedDo, Description = "Fixed do solfège", NoteLookup = new string[] {"Do","Do\u266f","Re","Mi\u266d","Mi","Fa","Fa\u266f","Sol","La\u266d","La","Si\u266d","Si"} }
                ,new MusicNotationType { Id = MusicNotationType.Enum.NorthernEuropean, Description = "Northern European", NoteLookup = new string[] {"C","Cis","D","Es","E","F","Fis","G","As","A","Bes","B"} }
            };

            _lookup = _values.ToDictionary((t)=>t.Id);
        }

        public static MusicNotationType[] Values  { get { return _values; } }

        public static MusicNotationType FromId (MusicNotationType.Enum id)
        {
            if(_lookup.ContainsKey(id)) {
                return _lookup[id];
            }
            throw new ArgumentOutOfRangeException("id",id,"in FromId()");
        }

        public static string ToDbString(this MusicNotationType obj)
        {
            return Convert.ToString(obj.Id).ToLowerInvariant();
        }

        public static MusicNotationType FromDbString(string str)
        {
            return FromId((MusicNotationType.Enum)Enum.Parse(typeof(MusicNotationType.Enum),str,true));
        }
    }
}
