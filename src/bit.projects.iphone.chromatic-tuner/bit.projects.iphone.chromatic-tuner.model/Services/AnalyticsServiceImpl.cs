using System;

using MonoTouch.Foundation;

using FlurryAnalytics;

using bit.shared.logging;

namespace bit.projects.iphone.chromatictuner.model
{
    public class AnalyticsServiceImpl : IAnalyticsService
    {
		private string _apiKey;

		public void StartSession()
		{
			Flurry.StartSession(_apiKey);
		}

		public void LogEvent(string eventName)
		{
			Flurry.LogEvent (eventName);
		}

        public bool InitialiseWithConfig(AnalyticsConfigSection config)
		{
			_apiKey = config.ApiKey;

			if (config.EnableDebug) {
				Flurry.SetDebugLog (true);
			}

			if (config.EnableCrashReporting) {
				Flurry.SetCrashReportingEnabled (true);
			}

			return true;
		}
    }    
}
