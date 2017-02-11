using System;

using MonoTouch.Foundation;

using bit.shared.logging;

namespace bit.projects.iphone.chromatictuner.model
{
    public interface IAppFeedbackService
    {
		string AppDisplayName { get; }
		int AppProductID { get; }

		bool InitialiseWithConfig(AppFeedbackConfigSection config);

		bool IncrementUsage();        

		void GotLove();
		void NoLove();

		void AppRatingGiven();
		void AppRatingOptOut();
		void AppRatingReminderWanted();	
		void AppFeedbackSent(string eventQualifier);
		void WebsiteVisited();

		bool IsTimeToShowRatingPrompt();
    }    
}
