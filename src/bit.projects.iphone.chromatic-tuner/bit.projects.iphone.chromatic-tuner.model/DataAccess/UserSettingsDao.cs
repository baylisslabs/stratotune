using System;
using SQLite;


namespace bit.projects.iphone.chromatictuner.model
{
    [Table("User_Settings")]
    public class UserSettingsDao
    {
        [PrimaryKey]
        [AutoIncrement]
        public int Id { get; set; }

        [Unique]      
        public string Name { get; set; }
        public bool   DisallowDelete { get; set; }       
        public double A4Calibration { get; set; }
        public string Notation_Id { get; set; }
        public string Transposition_Id { get; set; }
        public string Temperament_Id { get; set; }
        public string NeedleDamping_Id { get; set; }
        public bool   FrequencyDisplay { get; set; }
        public string PitchPipeWaveform_Id { get; set; }

        public UserSettingsDao ()
        {
        }

        public UserSettingsDao (UserSettings obj)
        {
            this.Id = obj.Id;
            this.Name = obj.Name;
            this.DisallowDelete = obj.DisallowDelete;
            this.A4Calibration = obj.A4Calibration;
            this.Notation_Id  = obj.Notation.ToDbString();
            this.Transposition_Id = obj.Transposition.ToDbString();
            this.Temperament_Id = obj.Temperament.ToDbString();
            this.NeedleDamping_Id = obj.NeedleDamping.ToDbString();
            this.FrequencyDisplay = obj.FrequencyDisplay;
            this.PitchPipeWaveform_Id = obj.PitchPipeWaveform.ToDbString();
        }

        public UserSettings ToObject()
        {
            return new UserSettings()
            {
                Id = this.Id,
                Name = this.Name,
                DisallowDelete = this.DisallowDelete,
                A4Calibration = this.A4Calibration,
                Notation = MusicNotationTypeExtensions.FromDbString(this.Notation_Id),
                Transposition = TranspositionTypeExtensions.FromDbString(this.Transposition_Id),
                Temperament = TemperamentTypeExtensions.FromDbString(this.Temperament_Id),
                NeedleDamping = NeedleDampingTypeExtensions.FromDbString(this.NeedleDamping_Id),
                FrequencyDisplay = this.FrequencyDisplay,
                PitchPipeWaveform = PitchPipeWaveformTypeExtensions.FromDbString(this.PitchPipeWaveform_Id)
            };
        }
    }
}

