using System;
using System.Collections.Generic;

using bit.shared.appconfig;

namespace bit.shared.logging
{
    public static class LogManager
    {
        private static LoggerConfiguration _logConfig;
        private static LoggerConfigSection _configSection;
        private static List<Logger> _loggers;
        private static object _lock = new object();

        public static LoggerConfiguration Configuration { get { return _logConfig; } set { _logConfig = value; } }
            
        static LogManager()
        {
            _configSection = new LoggerConfigSection();
            _logConfig = new LoggerConfiguration(_configSection);
            _loggers = new List<Logger>();
            AppConfigLocator.Get().RegisterForSection<LoggerConfigSection>(loadConfig);
        }
        
        public static Logger GetLogger (string name)
        {
            var logger = new Logger (name);
            logger.SetConfiguration (_logConfig);
            lock (_lock) {
                _loggers.Add(logger);
            }
            return logger;
        }      

        private static void loadConfig (LoggerConfigSection msg)
        {
            _configSection = msg;
            _logConfig = new LoggerConfiguration(_configSection);

            lock (_lock) {
                foreach(var l in _loggers) {
                    l.SetConfiguration(_logConfig);
                }
            }
        }
    }
}

