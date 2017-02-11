using System;
using System.Collections.Generic;
using System.Linq;

using MonoTouch.Foundation;
using MonoTouch.UIKit;

using bit.projects.iphone.chromatictuner.model;
using bit.shared.ios.msgbus;

namespace bit.projects.iphone.chromatictuner
{
    // The UIApplicationDelegate for the application. This class is responsible for launching the 
    // User Interface of the application, as well as listening (and optionally responding) to 
    // application events from iOS.
    [Register ("AppDelegate")]
    public partial class AppDelegate : UIApplicationDelegate
    {
        // class-level declarations
        private UIWindow window;
        private bit_projects_iphone_chromatic_tunerViewController rootViewController;
        private UINavigationController rootNavigationController;      
        private IUserSettingsService userSettings;
        private IFileSystemService fileSystem;
		private IAnalyticsService analytics;
		private IAppFeedbackService appFeedback;
        private IMsgBus msgBus;

        //
        // This method is invoked when the application has loaded and is ready to run. In this 
        // method you should instantiate the window, load the UI into it and then make the window
        // visible.
        //
        // You have 17 seconds to return from this method, or iOS will terminate your application.
        //
        public override bool FinishedLaunching (UIApplication app, NSDictionary options)
        {         
            var envConfig = new EnvConfig();
            envConfig.Initialise();
            var appConfig = new AppConfig(envConfig);
            appConfig.Initialise();
                      
            msgBus = new MsgBus("Main");
            fileSystem = new FileSystemServiceImpl();
            fileSystem.InitialiseWithConfig(appConfig.FileSystemConfig);
            userSettings = new UserSettingsServiceCached(
                new UserSettingsServiceImpl(appConfig.DbConfig.SETTINGS_DB,appConfig.DbConfig.SETTINGS_DB_CONFIGURE_PATH)
                );
            userSettings.Configure();

			analytics = new AnalyticsServiceImpl ();
			analytics.InitialiseWithConfig (appConfig.AnalyticsConfig);
			analytics.StartSession ();

			appFeedback = new AppFeedbackServiceImpl (analytics, userSettings);
			appFeedback.InitialiseWithConfig (appConfig.AppFeedbackConfig);

            new TunerAudioProcessor(msgBus,appConfig.TunerConfig);
            new AutoPitchPipe(msgBus);

            window = new UIWindow (UIScreen.MainScreen.Bounds);

            rootNavigationController = new UINavigationController();
            rootViewController = new bit_projects_iphone_chromatic_tunerViewController (
                msgBus,              
                appConfig.SupportConfig,
                userSettings,
				appFeedback);
            rootNavigationController.PushViewController(rootViewController,false);
            window.RootViewController = rootNavigationController;
            window.MakeKeyAndVisible ();
            
            return true;
        }

        public override void WillEnterForeground (UIApplication application)
        {
            if (rootViewController != null) {
                rootViewController.WillEnterForeground();
            }
        }

        public override void DidEnterBackground (UIApplication application)
        {
            if (rootViewController != null) {
                rootViewController.DidEnterBackground();
            }
        }


    }
}

