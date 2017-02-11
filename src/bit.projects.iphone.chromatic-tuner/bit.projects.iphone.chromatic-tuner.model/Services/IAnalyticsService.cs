using System;

using MonoTouch.Foundation;

using bit.shared.logging;

namespace bit.projects.iphone.chromatictuner.model
{
    public interface IAnalyticsService
    {
        bool InitialiseWithConfig(AnalyticsConfigSection config);

		void StartSession();
		void LogEvent(string eventName);
    }
    
}
