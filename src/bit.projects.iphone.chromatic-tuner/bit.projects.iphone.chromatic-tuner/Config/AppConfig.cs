using System;

using MonoTouch.Foundation;

using bit.shared.appconfig;
using bit.shared.logging;

using bit.projects.iphone.chromatictuner.model;

namespace bit.projects.iphone.chromatictuner
{
    public class AppConfig
    {
        static Logger _log = LogManager.GetLogger("AppConfig");

        private IAppConfig _appConfig;
        private IEnvConfig _envConfig;

        public TunerConfigSection TunerConfig { get { return _appConfig.GetSection<TunerConfigSection> (); } }
        public SupportConfigSection SupportConfig { get { return _appConfig.GetSection<SupportConfigSection> (); } }
        public DbConfigSection DbConfig { get { return _appConfig.GetSection<DbConfigSection> (); } }
        public FileSystemConfigSection FileSystemConfig { get { return _appConfig.GetSection<FileSystemConfigSection> (); } }
		public AnalyticsConfigSection AnalyticsConfig { get { return _appConfig.GetSection<AnalyticsConfigSection> (); } }
		public AppFeedbackConfigSection AppFeedbackConfig { get { return _appConfig.GetSection<AppFeedbackConfigSection> (); } }
		public LoggerConfigSection LoggerConfig { get { return _appConfig.GetSection<LoggerConfigSection> (); } }

        public AppConfig(IEnvConfig envConfig)
        {
            _appConfig = AppConfigLocator.Get (); 
            _envConfig = envConfig;
        }

        public void Initialise ()
        {                 
            _appConfig.SetConfigMask (getConfigMask ());
            loadConfigs ();
            if (_appConfig.AllSectionsInitialised) {
                _log.Debug ("AllSectionsInitialised");
            } else {
#if DEBUG
                throw new AppConfigException("AppConfig sections missing");              
#endif
            }
        }

        private ConfigMask getConfigMask()
        {
            var mask = new ConfigMask();
#if DEBUG
            mask.Add(ConfigFlag.BuildIsDebug);
#else
            mask.Add(ConfigFlag.BuildIsRelease);
#endif           
            return mask;
        }

        private void loadConfigs()
        {          
            _appConfig.LoadSection(ConfigFlag.BuildIsDebug, 
                                  new LoggerConfigSection {
                Trace = false, Debug = true, Info = true, Warn = true, Error = true, Fatal = true
            });

            _appConfig.LoadSection(ConfigFlag.BuildIsRelease, 
                                  new LoggerConfigSection {
                Trace = false, Debug = false, Info = false, Warn = false, Error = true, Fatal = true
            });
                     
            _appConfig.LoadSection(ConfigFlag.Any,
                                  new TunerConfigSection {
                NumInputPackets = 3600,
                NumInputBuffers = 3,
                NumOutputPackets = 1024,
                NumOutputBuffers = 3,
                SamplingRate = 44100,
                PitchPipeGain = 0.5
            });

            _appConfig.LoadSection(ConfigFlag.Any,
                                   new SupportConfigSection {
                UrlWebsite = @"http://www.baylisslabs.com/stratotune",
                UrlReview = @"http://itunes.apple.com/app/id579888710",
				SupportEmail = @"support@baylisslabs.com"
            });

            _appConfig.LoadSection(ConfigFlag.Any,
                                   new FileSystemConfigSection {
                FoldersToCreate = _envConfig.Subst(@"${APP_SUPPORT_FOLDER}/bit.projects.iphone.chromatictuner")
            });

            _appConfig.LoadSection(ConfigFlag.Any,
                                   new DbConfigSection {
                SETTINGS_DB = _envConfig.Subst(@"${APP_SUPPORT_FOLDER}/bit.projects.iphone.chromatictuner/Settings.0.db"),
                SETTINGS_DB_CONFIGURE_PATH = @"./Content/Dba/SETTINGS_DB_0_CONFIGURE.sql"
            });

			_appConfig.LoadSection(ConfigFlag.BuildIsDebug,
			                       new AnalyticsConfigSection {
				ApiKey = "3QS3VM4ZT4D7ZQ9NMXX4", EnableDebug = true, EnableCrashReporting = true
			});

			_appConfig.LoadSection(ConfigFlag.BuildIsRelease,
			                       new AnalyticsConfigSection {
				ApiKey = "H7J54RZFTH7GZ3H9SNGC", EnableDebug = false, EnableCrashReporting = true
			});

			_appConfig.LoadSection(ConfigFlag.BuildIsDebug,
			                       new AppFeedbackConfigSection {
				AppDisplayName = "Stratotune", 
				AppProductID = 579888710,
				DaysUntilPrompt = 0.25/(24*60),
				UsesUntilPrompt = 3,
				SignificantEventsUntilPrompt = 0,
				TimeBeforeRemindingDays = 0.25/(24*60),
				DebugMode = true
			});

			_appConfig.LoadSection(ConfigFlag.BuildIsRelease,
			                       new AppFeedbackConfigSection {
				AppDisplayName = "Stratotune", 
				AppProductID = 579888710,
				DaysUntilPrompt = 21,
				UsesUntilPrompt = 5,
				SignificantEventsUntilPrompt = 0,
				TimeBeforeRemindingDays = 5
			});
		}		      
    }
}

