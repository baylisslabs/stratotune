using System;
using System.Collections.Generic;

namespace bit.shared.appconfig
{
    public class ConfigMask : IEqualityComparer<ConfigFlag>
    {
        private HashSet<ConfigFlag> _flags;

        public ConfigMask ()
        {
            _flags = new HashSet<ConfigFlag>();
        }

        public ConfigMask(ConfigFlag flag)
        {
            _flags = new HashSet<ConfigFlag>();
            _flags.Add(flag);
        }

        public ConfigMask(params ConfigFlag[] flags)
        {
            _flags = new HashSet<ConfigFlag>(flags);
        }

        public void Add(params ConfigFlag[] flags)
        {
            _flags.UnionWith(flags);
        }

        public bool Contains(ConfigFlag flag)
        {
            return _flags.Contains(flag);
        }

        public bool ContainsAll(ConfigMask mask)
        {
            return _flags.IsSupersetOf(mask._flags);
        }               

        public static implicit operator ConfigMask(ConfigFlag flag)
        {
            return new ConfigMask(flag);
        }

        #region IEqualityComparer implementation

        bool IEqualityComparer<ConfigFlag>.Equals (ConfigFlag x, ConfigFlag y)
        {
            return x.Id == y.Id;
        }

        int IEqualityComparer<ConfigFlag>.GetHashCode (ConfigFlag obj)
        {
            return obj.Id.GetHashCode();
        }

        #endregion
    }
}

