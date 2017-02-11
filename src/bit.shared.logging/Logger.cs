using System;

namespace bit.shared.logging
{
    public class Logger
    {                        
        private volatile bool isTraceEnabled;
        private volatile bool isDebugEnabled;
        private volatile bool isInfoEnabled;
        private volatile bool isWarnEnabled;
        private volatile bool isErrorEnabled;
        private volatile bool isFatalEnabled;
        private volatile LoggerConfiguration configuration;

        protected internal Logger (string name)
        {
            this.Name = name;
        }
        
           /// <summary>
        /// Gets the name of the logger.
        /// </summary>
        public string Name { get; private set; }
      
        /// <summary>
        /// Gets a value indicating whether logging is enabled for the <c>Trace</c> level.
        /// </summary>
        /// <returns>A value of <see langword="true" /> if logging is enabled for the <c>Trace</c> level, otherwise it returns <see langword="false" />.</returns>
        public bool IsTraceEnabled
        {
            get { return this.isTraceEnabled; }
        }

        /// <summary>
        /// Gets a value indicating whether logging is enabled for the <c>Debug</c> level.
        /// </summary>
        /// <returns>A value of <see langword="true" /> if logging is enabled for the <c>Debug</c> level, otherwise it returns <see langword="false" />.</returns>
        public bool IsDebugEnabled
        {
            get { return this.isDebugEnabled; }
        }

        /// <summary>
        /// Gets a value indicating whether logging is enabled for the <c>Info</c> level.
        /// </summary>
        /// <returns>A value of <see langword="true" /> if logging is enabled for the <c>Info</c> level, otherwise it returns <see langword="false" />.</returns>
        public bool IsInfoEnabled
        {
            get { return this.isInfoEnabled; }
        }

        /// <summary>
        /// Gets a value indicating whether logging is enabled for the <c>Warn</c> level.
        /// </summary>
        /// <returns>A value of <see langword="true" /> if logging is enabled for the <c>Warn</c> level, otherwise it returns <see langword="false" />.</returns>
        public bool IsWarnEnabled
        {
            get { return this.isWarnEnabled; }
        }

        /// <summary>
        /// Gets a value indicating whether logging is enabled for the <c>Error</c> level.
        /// </summary>
        /// <returns>A value of <see langword="true" /> if logging is enabled for the <c>Error</c> level, otherwise it returns <see langword="false" />.</returns>
        public bool IsErrorEnabled
        {
            get { return this.isErrorEnabled; }
        }

        /// <summary>
        /// Gets a value indicating whether logging is enabled for the <c>Fatal</c> level.
        /// </summary>
        /// <returns>A value of <see langword="true" /> if logging is enabled for the <c>Fatal</c> level, otherwise it returns <see langword="false" />.</returns>
        public bool IsFatalEnabled
        {
            get { return this.isFatalEnabled; }
        }
        
        public void Trace(LogMessageGenerator lmg)
        {
            if(isTraceEnabled)
            {
                writeToTargets(LogLevel.Trace,lmg());
            }
        }
        
        public void Trace(string msg, params object[] args)
        {
            if(isTraceEnabled)
            {
                writeToTargets(LogLevel.Trace,String.Format(msg,args));
            }
        }
        
        public void Debug(LogMessageGenerator lmg)
        {
            if(isDebugEnabled)
            {
                writeToTargets(LogLevel.Debug,lmg());
            }
        }
        
        public void Debug(string msg, params object[] args)
        {
            if(isDebugEnabled)
            {
                writeToTargets(LogLevel.Debug,String.Format(msg,args));
            }
        }
        
        public void Info(LogMessageGenerator lmg)
        {
            if(isInfoEnabled)
            {
                writeToTargets(LogLevel.Info,lmg());
            }
        }
        
        public void Info(string msg, params object[] args)
        {
            if(isInfoEnabled)
            {
                writeToTargets(LogLevel.Info,String.Format(msg,args));
            }
        }
        
        public void Warn(LogMessageGenerator lmg)
        {
            if(isWarnEnabled)
            {
                writeToTargets(LogLevel.Warn,lmg());
            }
        }
        
        public void Warn(string msg, params object[] args)
        {
            if(isWarnEnabled)
            {
                writeToTargets(LogLevel.Warn,String.Format(msg,args));
            }
        }   
        
        public void Error(LogMessageGenerator lmg)
        {
            if(isErrorEnabled)
            {
                writeToTargets(LogLevel.Error,lmg());
            }
        }
        
        public void Error(string msg, params object[] args)
        {
            if(isErrorEnabled)
            {
                writeToTargets(LogLevel.Error,String.Format(msg,args));
            }
        } 

        public void Error(string msg, Exception exception)
        {
            if(isErrorEnabled)
            {
                writeToTargets(LogLevel.Error,msg,exception);
            }
        } 
        
       public void Fatal(LogMessageGenerator lmg)
        {
            if(isTraceEnabled)
            {
                writeToTargets(LogLevel.Fatal,lmg());
            }
        }
        
        public void Fatal(string msg, params object[] args)
        {
            if(isFatalEnabled)
            {
                writeToTargets(LogLevel.Fatal,String.Format(msg,args));
            }
        }        

        public void Fatal(string msg, Exception exception)
        {
            if(isFatalEnabled)
            {
                writeToTargets(LogLevel.Fatal,msg,exception);
            }
        } 
        
        private void writeToTargets(LogLevel logLevel, string message)
        {
            LoggerImpl.Write(getTargetsForLevel(logLevel), new LogEventInfo { Level = logLevel, Source = this.Name, Message = message });
        }

        private void writeToTargets(LogLevel logLevel, string message, Exception exception)
        {
            LoggerImpl.Write(getTargetsForLevel(logLevel), new LogEventInfo { Level = logLevel, Source = this.Name, Message = message, Exception = exception });
        }
        
        internal void SetConfiguration(LoggerConfiguration newConfiguration)
        {           
            // pre-calculate 'enabled' flags
            this.configuration = newConfiguration;
            this.isTraceEnabled = newConfiguration.IsEnabled(LogLevel.Trace);
            this.isDebugEnabled = newConfiguration.IsEnabled(LogLevel.Debug);
            this.isInfoEnabled = newConfiguration.IsEnabled(LogLevel.Info);
            this.isWarnEnabled = newConfiguration.IsEnabled(LogLevel.Warn);
            this.isErrorEnabled = newConfiguration.IsEnabled(LogLevel.Error);
            this.isFatalEnabled = newConfiguration.IsEnabled(LogLevel.Fatal);           
        }

        private Target getTargetsForLevel(LogLevel level)
        {
            return this.configuration.GetTargetsForLevel(level);
        }
    }
}

