using System;
using System.Collections.Generic;

namespace bit.shared.appconfig
{
    internal class AppConfigImpl : IAppConfig
    {
        ConfigMask _configMask;
        Dictionary<string,SectionAdapter> _handlers;

        public AppConfigImpl ()
        {
            _configMask = new ConfigMask();
            _handlers = new Dictionary<string, SectionAdapter>();
        }

        #region IAppConfig implementation

        public bool AllSectionsInitialised 
        {
            get 
            {
                foreach(var h in _handlers.Values) {
                    if(!h.AllCalled()) {
                        return false;
                    }
                }
                return true;
            }
        }

        public void SetConfigMask (ConfigMask mask)
        {
            _configMask = mask;
        }


        public void RegisterForSection<T> (Action<T> handler) where T : ConfigSection, new()
        {
            var name = new T().Name;
            if (!_handlers.ContainsKey (name)) {
                _handlers.Add(name, new SectionAdapter());
            }
            _handlers[name].AddHandler(handler);
        }

        public T GetSection<T>() where T : ConfigSection, new()
        {
            var name = new T().Name;
            if (!_handlers.ContainsKey (name)) {
                throw new AppConfigException(String.Format("{0} section not found",name));
            }
            var msg = _handlers[name].GetMessage<T>();
            if(msg==null) {
                throw new AppConfigException(String.Format("{0} section not found",name));
            }
            return msg;
        }


        public void LoadSection<T>(ConfigMask mask, T configData) where T : ConfigSection
        {
            if (mask.Contains(ConfigFlag.Any) || _configMask.ContainsAll (mask)) {
                if (!_handlers.ContainsKey (configData.Name)) {
                    _handlers.Add(configData.Name, new SectionAdapter());
                }               
                _handlers[configData.Name].CallHandlers(configData);
            }
        }

        #endregion
    }
}

