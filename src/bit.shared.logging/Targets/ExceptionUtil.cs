using System;

namespace bit.shared.logging
{
    public static class ExceptionUtil
    {       
        public static string ExceptionToStr(Exception ex)
        {
            return String.Format("{0} {1} {2}",typeof(Exception).FullName,ex.Message,ex.StackTrace);
        }
    }
}

