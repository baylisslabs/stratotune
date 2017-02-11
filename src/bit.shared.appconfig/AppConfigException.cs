using System;

namespace bit.shared.appconfig
{
    public class AppConfigException : Exception
    {
        public AppConfigException (string msg) : base(msg)
        {
        }
    }
}

