using System;

namespace bit.shared.logging
{
    public class ConsoleOutTarget : Target
    {
        public ConsoleOutTarget ()
        {
        }

        #region Target implementation

        public void Write (LogEventInfo info)
        {
            if (info.Exception == null) {
                Console.WriteLine (
                    String.Format ("{0} {1} {2}", info.Level, info.Source, info.Message));
            } 
            else {
                Console.WriteLine (
                    String.Format ("{0} {1} {2} {3}", info.Level, info.Source, info.Message, ExceptionUtil.ExceptionToStr(info.Exception)));
            }
        }

        #endregion
    }
}

