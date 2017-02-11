using System;
using System.Collections.Generic;

namespace bit.shared.appconfig
{
    public abstract class EnvConfigBase : IEnvConfig
    {                          
        Dictionary <string,string> _vars;

        public EnvConfigBase()
        {
            _vars = new Dictionary<string, string>();
        }

        public string Get (string key)
        {
            if (_vars.ContainsKey (key)) {
                return _vars[key];
            }
            return null;
        }

        public void Set(string key,string value)
        {
            _vars[key] = value;
        }

        //
        // replace occurences of ${env-key} with value
        //
        public string Subst (string str)
        {
            // TODO: optimise this
            var result = str;
            foreach (var kvp in _vars) {
                if(kvp.Value!=null) {
                    result = result.Replace("${"+kvp.Key+"}",kvp.Value);
                }
            }
            return result;
        }
    }    
}
