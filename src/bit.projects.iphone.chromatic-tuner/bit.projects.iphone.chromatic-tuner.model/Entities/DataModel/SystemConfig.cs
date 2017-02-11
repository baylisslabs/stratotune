using System;
using SQLite;


namespace bit.projects.iphone.chromatictuner.model
{
    public class SystemConfig
    {      
        public int Id { get; set; }
        public int CurrentUserSetting_Id { get; set; }
        public int DefaultUserSetting_Id { get; internal set; }
        public bool HeadPhoneAlertDisabled { get; set; }
    }
}

