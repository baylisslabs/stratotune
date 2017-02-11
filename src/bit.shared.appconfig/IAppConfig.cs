using System;

namespace bit.shared.appconfig
{
    public interface IAppConfig
    {
        void SetConfigMask(ConfigMask mask);
        void RegisterForSection<T>(Action<T> handler) where T : ConfigSection, new();
        void LoadSection<T>(ConfigMask mask, T configData) where T : ConfigSection;
        T GetSection<T>() where T : ConfigSection, new();
        bool AllSectionsInitialised { get; }
    }
}

