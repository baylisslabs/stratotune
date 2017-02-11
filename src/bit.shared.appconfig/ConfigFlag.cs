using System;

namespace bit.shared.appconfig
{
    public class ConfigFlag
    {
        public static ConfigFlag Any = new ConfigFlag("Any");
        public static ConfigFlag BuildIsDebug = new ConfigFlag("BuildIsDebug");
        public static ConfigFlag BuildIsRelease = new ConfigFlag("BuildIsRelease");

        public string Id { get; private set; }

        public ConfigFlag (string id)
        {
            this.Id = id;
        }
               
        public static ConfigMask operator|(ConfigFlag a, ConfigFlag b)
        {
            var c = new ConfigMask(a);
            c.Add(b);
            return c;
        }
    }
}

