using System;

using bit.shared.appconfig;

namespace bit.projects.iphone.chromatictuner.model
{
	public class AppFeedbackConfigSection : ConfigSection
    {
		public override string Name { get { return "bit.projects.iphone.chromatictuner.AppFeedbackConfigSection"; } }

		public string AppDisplayName { get; set; }
		public int AppProductID { get; set; }
		public double DaysUntilPrompt { get; set; }
		public int UsesUntilPrompt { get; set; }
		public int SignificantEventsUntilPrompt { get; set; }
		public double TimeBeforeRemindingDays { get; set; }
		public bool DebugMode { get; set; }
    }
}

