using System;

namespace bit.shared.logging
{
    public struct LogEventInfo
    {
        public LogLevel Level { get; set; }
        public string Source { get; set; }
        public string Message { get; set; }  
        public Exception Exception { get; set; }

    }
}

