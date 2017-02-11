using System;
using System.Collections.Generic;

namespace bit.shared.appconfig
{
    public interface IEnvConfig
    {       
        string Get (string key);
        string Subst (string str);
        void Set(string key,string value);
    }
}

