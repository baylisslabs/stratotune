using System;
using System.Collections.Generic;
using System.Linq;

using MonoTouch.Foundation;
using MonoTouch.UIKit;
using MonoTouch.AudioToolbox;

namespace Hello_MultiScreen_iPhone
{
	// The UIApplicationDelegate for the application. This class is responsible for launching the 
	// User Interface of the application, as well as listening (and optionally responding) to 
	// application events from iOS.
	[Register ("AppDelegate")]
	public partial class AppDelegate : UIApplicationDelegate
	{
		// class-level declarations
		private UIWindow _window;
		private NSTimer _timer;
		private HomeScreen _homeScreen;
		private UINavigationController _rootNavigationController;

		//
		// This method is invoked when the application has loaded and is ready to run. In this 
		// method you should instantiate the window, load the UI into it and then make the window
		// visible.
		//
		// You have 17 seconds to return from this method, or iOS will terminate your application.
		//
		public override bool FinishedLaunching (UIApplication app, NSDictionary options)
		{
			// create a new window instance based on the screen size
			_window = new UIWindow (UIScreen.MainScreen.Bounds);	

			_rootNavigationController = new UINavigationController();
			_homeScreen = new HomeScreen();

			_rootNavigationController.PushViewController(_homeScreen,false);
			_window.RootViewController = _rootNavigationController;
			_timer = NSTimer.CreateRepeatingScheduledTimer(TimeSpan.FromMilliseconds(100), onTimer);

			AudioSession.Initialize();
			AudioSession.SetActive(true);
			AudioSession.Category = AudioSessionCategory.RecordAudio;

			// make the window visible
			_window.MakeKeyAndVisible ();

			return true;
		}

		public override void WillTerminate (UIApplication application)
		{
			if (_timer != null) {
				_timer.Invalidate();
				_timer = null;
			}
		}

		private void onTimer ()
		{
			if (_homeScreen != null) {
				_homeScreen.OnTimer();
			}
		}
	}
}

