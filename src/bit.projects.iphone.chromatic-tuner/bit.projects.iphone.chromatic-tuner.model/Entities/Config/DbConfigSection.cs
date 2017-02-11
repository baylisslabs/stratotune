using System;

using bit.shared.appconfig;

namespace bit.projects.iphone.chromatictuner.model
{
    public class DbConfigSection : ConfigSection
    {
        public override string Name { get { return "bit.projects.iphone.chromatictuner.DbConfigSection"; } }

        public string SETTINGS_DB { get; set; }
        public string SETTINGS_DB_CONFIGURE_PATH { get; set; }
    }
}

