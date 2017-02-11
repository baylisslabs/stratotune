using System;

using MonoTouch.Foundation;
using MonoTouch.UIKit;
using MonoTouch.MessageUI;

using bit.shared.logging;
using bit.projects.iphone.chromatictuner.model;

namespace bit.projects.iphone.chromatictuner
{
	public class AppRatingFlow
	{
		static Logger _log = LogManager.GetLogger("AppRatingFlow");

		private readonly string _appDisplayName;
		private readonly int _appID;
		private bool _canLeaveFeedback;

		private readonly SupportConfigSection _supportConfig;
		private readonly IAppFeedbackService _appFeedbackService;

		private UIViewController _viewController;
		private bool _inProgress;

		public AppRatingFlow (IAppFeedbackService appFeedbackService, SupportConfigSection supportConfig)
		{
			_appFeedbackService = appFeedbackService;
			_supportConfig = supportConfig;

			_appDisplayName = _appFeedbackService.AppDisplayName;
			_appID = _appFeedbackService.AppProductID;

			_canLeaveFeedback = MFMailComposeViewController.CanSendMail;
		}

		public void SetViewController(UIViewController viewController)
		{
			_viewController = viewController;
		}

		public void ShowRatingFlow()
		{
			_log.Debug ("ShowRatingFlow()");

			if (!_inProgress && _viewController != null) {
				_inProgress = true;
				askForLove ();
			}		
		}

		private void askForLove()
		{
			var doYouLoveMeView = buildDoYouLoveMePrompt ();
			doYouLoveMeView.Show ();
		}

		private void gotLove()
		{
			_appFeedbackService.GotLove ();
			askForRating ();
		}

		private void noLove()
		{
			_appFeedbackService.NoLove ();
			askForFeedback ();
		}

		private void askForRating()
		{
			var askForRatingView = buildAskForRatingPrompt ();
			askForRatingView.Show ();
		}

		private void askForFeedback()
		{
			if (_canLeaveFeedback) {
				var askForFeedbackView = buildAskForFeedbackPrompt ();
				askForFeedbackView.Show ();
			} else {
				var askForFeedbackView = buildAskForFeedbackPromptNoMail ();
				askForFeedbackView.Show ();
			}
		}

		private void presentFeedbackForm()
		{
			var _mailController = new MFMailComposeViewController ();
			_mailController.SetToRecipients (new string[]{_supportConfig.SupportEmail});
			_mailController.SetSubject ("Re: We're Sorry");
			_mailController.SetMessageBody (
				string.Format("> What can we do to ensure that you love {0}? We appreciate your constructive feedback.",_appDisplayName),
				false);
			_mailController.Finished += handleComposeMailFinished;
			if (_viewController != null) {
				_viewController.PresentViewController (_mailController, true, null);
			}
		}

		private void presentWebsite()
		{
			_appFeedbackService.WebsiteVisited ();
			UIApplication.SharedApplication.OpenUrl(new NSUrl(_supportConfig.UrlWebsite));
			_inProgress = false;
		}

		private void gotoStoreAndRate()
		{
			_appFeedbackService.AppRatingGiven ();

			var url = string.Format (@"http://itunes.apple.com/app/id{0}", _appID);
			UIApplication.SharedApplication.OpenUrl (new NSUrl (url));
			_inProgress = false;
		}

		private void remindMeLater()
		{
			_appFeedbackService.AppRatingReminderWanted ();
			_inProgress = false;
		}

		private void noThanks()
		{
			_appFeedbackService.AppRatingOptOut ();
			_inProgress = false;
		}

		private void handleDoYouLoveMeButtonClicked(object sender, UIButtonEventArgs e)
		{
			if (e.ButtonIndex == 1) {
				gotLove ();
			} else {
				noLove ();
			}
		}

		private void handleAskForRatingButtonClicked(object sender, UIButtonEventArgs e)
		{
			if (e.ButtonIndex == 1) {
				gotoStoreAndRate ();
			} else if (e.ButtonIndex == 2) {
				remindMeLater ();
			} else {
				noThanks ();
			}
		}

		private void handleLeaveFeedbackButtonClicked(object sender, UIButtonEventArgs e)
		{
			if (e.ButtonIndex == 1) {
				presentFeedbackForm ();
			} else {
				noThanks ();
			}
		}

		private void handleVisitWebsiteButtonClicked(object sender, UIButtonEventArgs e)
		{
			if (e.ButtonIndex == 1) {
				presentWebsite ();
			} else {
				noThanks ();
			}
		}

		private void handleComposeMailFinished(object sender, MFComposeResultEventArgs e)
		{
			_appFeedbackService.AppFeedbackSent (e.Result.ToString ());
			e.Controller.DismissViewController (true, null);
			_inProgress = false;
		}

		private UIAlertView buildDoYouLoveMePrompt()
		{
			var title = string.Format ("Do you love {0}?", _appDisplayName);
			var view = new UIAlertView (title, "", null, "No", new [] { "Yes" });
			view.Clicked += handleDoYouLoveMeButtonClicked;
			return view;
		}

		private UIAlertView buildAskForRatingPrompt()
		{
			var msg = string.Format (@"We're so happy to hear that you love {0}! It'd be really helpful if you rated us. Thanks so much for spending some time with us.",
			                          _appDisplayName);

			var rateButtonLabel = string.Format ("Rate {0}", _appDisplayName);
			var view = new UIAlertView ("Thank You", msg,	null, "No Thanks", new [] { rateButtonLabel, "Remind Me Later" });
			view.Clicked += handleAskForRatingButtonClicked;
			return view;
		}

		private UIAlertView buildAskForFeedbackPrompt()
		{
			var msg = @"What can we do to ensure that you love our app? We'd appreciate your constructive feedback.";

			var view = new UIAlertView ("We're Sorry!", msg, null, "No Thanks", new [] { "Leave Feedback" });
			view.Clicked += handleLeaveFeedbackButtonClicked;
			return view;
		}

		private UIAlertView buildAskForFeedbackPromptNoMail()
		{
			var msg = @"We'd love to hear any feedback you'd like to share with us. Please visit our website to get in touch.";

			var view = new UIAlertView ("We're Sorry!", msg, null, "No Thanks", new [] { "Visit Website" });
			view.Clicked += handleVisitWebsiteButtonClicked;
			return view;
		}
	}
}

