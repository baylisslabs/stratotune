using System;

namespace bit.shared.appconfig
{
    public static class AppConfigLocator
    {
        private static IAppConfig _configInstance;

        static AppConfigLocator ()
        {
            _configInstance = new AppConfigImpl();
        }

        public static IAppConfig Get()
        {
            return _configInstance;
        }
    }
}

