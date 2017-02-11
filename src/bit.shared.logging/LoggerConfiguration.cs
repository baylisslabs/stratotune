using System;
using System.Collections.Generic;

namespace bit.shared.logging
{
    public class LoggerConfiguration
    {
        private Dictionary<LogLevel,Target> _targets;
                        
        public LoggerConfiguration ()
        {
            _targets = new Dictionary<LogLevel, Target>();
        }

        public LoggerConfiguration (LoggerConfigSection configSection)
        {
            _targets = new Dictionary<LogLevel, Target>();
            if (configSection.Trace) _targets[LogLevel.Trace] = new ConsoleOutTarget();
            if (configSection.Debug) _targets[LogLevel.Debug] = new ConsoleOutTarget();
            if (configSection.Info) _targets[LogLevel.Info] = new ConsoleOutTarget();
            if (configSection.Warn) _targets[LogLevel.Warn] = new ConsoleErrorTarget();
            if (configSection.Error) _targets[LogLevel.Error] = new ConsoleErrorTarget();
            if (configSection.Fatal) _targets[LogLevel.Fatal] = new ConsoleErrorTarget();
        }

        public bool IsEnabled(LogLevel level)
        {
            return _targets.ContainsKey(level) && _targets[level] != null;
        }

        public Target GetTargetsForLevel (LogLevel level)
        {
            if (_targets.ContainsKey (level)) {
                return _targets[level];
            }
            return null;
        }
    }
}

