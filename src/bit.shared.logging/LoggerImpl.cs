using System;

namespace bit.shared.logging
{
    internal static class LoggerImpl
    {
       internal static void Write (Target target, LogEventInfo info)
        {
            try {
                if (target != null) {
                    target.Write (info);
                }
            }
            catch (Exception) {
                // prevent logging exceptions from propagating
            }
       }               
    }
}

