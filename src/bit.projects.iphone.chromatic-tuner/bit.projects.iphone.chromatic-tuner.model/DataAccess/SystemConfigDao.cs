using System;
using SQLite;


namespace bit.projects.iphone.chromatictuner.model
{
    [Table("System_Config")]
    public class SystemConfigDao
    {
        [PrimaryKey]       
        public int Id { get; set; }
        public int CurrentUserSetting_Id { get; set; }
        public int DefaultUserSetting_Id { get; set; }
        public bool HeadPhoneAlertDisabled { get; set; }

        public SystemConfigDao ()
        {
        }

        public SystemConfigDao (SystemConfig obj)
        {
            this.Id = obj.Id;
            this.CurrentUserSetting_Id = obj.CurrentUserSetting_Id;
            this.DefaultUserSetting_Id = obj.DefaultUserSetting_Id;
            this.HeadPhoneAlertDisabled = obj.HeadPhoneAlertDisabled;
        }
        
        public SystemConfig ToObject()
        {
            return new SystemConfig()
            {
                Id = this.Id,
                CurrentUserSetting_Id = this.CurrentUserSetting_Id,
                DefaultUserSetting_Id = this.DefaultUserSetting_Id,
                HeadPhoneAlertDisabled = this.HeadPhoneAlertDisabled
            };
        }
    }
}

