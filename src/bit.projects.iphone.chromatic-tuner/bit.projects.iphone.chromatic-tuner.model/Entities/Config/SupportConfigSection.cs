using System;

using bit.shared.appconfig;

namespace bit.projects.iphone.chromatictuner.model
{
    public class SupportConfigSection : ConfigSection
    {
        public override string Name { get { return "bit.projects.iphone.chromatictuner.SupportConfigSection"; } }

        public string UrlWebsite { get; set; }
        public string UrlReview { get; set; }       
		public string SupportEmail { get; set; }
    }
}

