using System;

using MonoTouch.Foundation;
using MonoTouch.SystemConfiguration;

using bit.shared.logging;

namespace bit.projects.iphone.chromatictuner.model
{
    public class AppFeedbackServiceImpl : IAppFeedbackService
    {
		static Logger _log = LogManager.GetLogger("AppFeedbackServiceImpl");

		private AppFeedbackConfigSection _config;
		private IAnalyticsService _analyticsService;
		private IUserSettingsService _userSettings;

		public string AppDisplayName { get { return _config.AppDisplayName; } }
		public int AppProductID { get { return _config.AppProductID; } }

		public AppFeedbackServiceImpl(IAnalyticsService analyticsService, IUserSettingsService userSettings)
		{
			_analyticsService = analyticsService;
			_userSettings = userSettings;
		}

		public bool InitialiseWithConfig(AppFeedbackConfigSection config)
		{
			_config = config;
			return true;
		}

		public bool IncrementUsage()
		{
			_log.Debug ("IncrementUsage()");

			var appFeedback = _userSettings.GetAppFeedback ();		
			var currentVersion = getCurrentProductVersion ();

			_log.Debug(()=>toString(appFeedback));

			if (appFeedback!=null && currentVersion!=null) {
				if (appFeedback.VersionAtLastUsage == null || appFeedback.VersionAtLastUsage != currentVersion) {
					if (appFeedback.RatingFlowCompleted) {
						resetUsage (appFeedback, currentVersion);
					}
				}
				appFeedback.UsesCount++;
				_userSettings.UpdateAppFeedback (appFeedback);
			}
					

			return isTimeToShowRatingPromptInternal (appFeedback) || _config.DebugMode;
		}

		public void GotLove()
		{
			_analyticsService.LogEvent ("AppFeedbackService.GotLove");
		}

		public void NoLove()
		{
			_analyticsService.LogEvent ("AppFeedbackService.NoLove");
		}

		public void AppRatingGiven()
		{
			setRatingFlowCompleted ();
			_analyticsService.LogEvent ("AppFeedbackService.AppRatingGiven");
		}

		public void AppRatingOptOut()
		{
			setRatingFlowCompleted ();
			_analyticsService.LogEvent ("AppFeedbackService.AppRatingOptOut");
		}

		public void AppRatingReminderWanted()
		{
			setReminder (_config.TimeBeforeRemindingDays);
			_analyticsService.LogEvent ("AppFeedbackService.AppRatingReminderWanted");
		}

		public void AppFeedbackSent(string eventQualifier)
		{
			setRatingFlowCompleted ();
			_analyticsService.LogEvent ("AppFeedbackService.AppFeedbackSent."+eventQualifier);
		}

		public void WebsiteVisited()
		{
			setRatingFlowCompleted ();
			_analyticsService.LogEvent ("AppFeedbackService.WebsiteVisited.");
		}

		public bool IsTimeToShowRatingPrompt()
		{
			_log.Debug ("IsTimeToShowRatingPrompt()");

			var appFeedback = _userSettings.GetAppFeedback ();
			return isTimeToShowRatingPromptInternal (appFeedback) || _config.DebugMode;
		}

		private void setReminder(double days)
		{
			var appFeedback = _userSettings.GetAppFeedback ();
			if (appFeedback != null) {
				appFeedback.DoNotRemindBeforeTime = DateTime.UtcNow + TimeSpan.FromDays (days);
				_userSettings.UpdateAppFeedback (appFeedback);
			}
		}

		private void setRatingFlowCompleted()
		{
			var appFeedback = _userSettings.GetAppFeedback ();
			if (appFeedback != null) {
				appFeedback.RatingFlowCompleted = true;
				_userSettings.UpdateAppFeedback (appFeedback);
			}
		}

		private bool isTimeToShowRatingPromptInternal(AppFeedback appFeedback)
		{
			// todo: only show if network reachable
			return appFeedback != null
				&& appFeedback.RatingFlowCompleted == false
					&& _config.DaysUntilPrompt <= getDaysSinceInstall (appFeedback)
					&& _config.UsesUntilPrompt <= appFeedback.UsesCount
					&& _config.SignificantEventsUntilPrompt <= appFeedback.SignificantUsesCount
					&& getDaysPastReminderTime (appFeedback) >= 0;
		}

		private void resetUsage(AppFeedback appFeedback, string newVersion)
		{
			appFeedback.VersionAtLastUsage = newVersion;
			appFeedback.VersionFirstUsedTimeStamp = DateTime.UtcNow;
			appFeedback.UsesCount = 0;
			appFeedback.SignificantUsesCount = 0;
			appFeedback.DoNotRemindBeforeTime = null;
			appFeedback.RatingFlowCompleted = false;
		}

		private double getDaysSinceInstall(AppFeedback appFeedback)
		{
			if (appFeedback.VersionFirstUsedTimeStamp.HasValue) {
				return (DateTime.UtcNow - appFeedback.VersionFirstUsedTimeStamp.Value).TotalDays;
			}
			return 0;
		}

		private double getDaysPastReminderTime(AppFeedback appFeedback)
		{
			if (appFeedback.DoNotRemindBeforeTime.HasValue) {
				return (DateTime.UtcNow - appFeedback.DoNotRemindBeforeTime.Value).TotalDays;
			}
			return 0;
		}

		private string getCurrentProductVersion()
		{
			var cv = NSBundle.MainBundle.InfoDictionary ["CFBundleVersion"].ToString();
			_log.Debug ("CurrentVersion: {0}", cv ?? "null");
			return cv;
		}

		private string toString(AppFeedback appFeedback)
		{
			if (appFeedback != null) {
				return String.Format ("{{VersionAtLastUsage:{0},VersionFirstUsedTimeStamp:{1},UsesCount:{2},SignificantUsesCount:{3},DoNotRemindBeforeTime:{4},RatingFlowCompleted:{5}}}",
				                     appFeedback.VersionAtLastUsage,
				                     appFeedback.VersionFirstUsedTimeStamp.GetValueOrDefault (),
				                     appFeedback.UsesCount,
				                     appFeedback.SignificantUsesCount,
				                     appFeedback.DoNotRemindBeforeTime.GetValueOrDefault (),
				                     appFeedback.RatingFlowCompleted);
			}
			return "null";
		}
    }    
}
