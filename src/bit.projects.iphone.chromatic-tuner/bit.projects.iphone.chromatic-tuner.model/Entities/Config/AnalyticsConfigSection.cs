using System;

using bit.shared.appconfig;

namespace bit.projects.iphone.chromatictuner.model
{
	public class AnalyticsConfigSection : ConfigSection
    {
		public override string Name { get { return "bit.projects.iphone.chromatictuner.AnalyticsConfigSection"; } }

		public string ApiKey { get; set; }
		public bool EnableDebug { get; set; }
		public bool EnableCrashReporting { get; set; }
    }
}

