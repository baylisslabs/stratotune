using System;
using System.Collections.Generic;

namespace bit.shared.appconfig
{
    internal class SectionAdapter
    {
        List<SectionHandler> _handlers;       
        Dictionary<string,ConfigSection> _messages;

        public SectionAdapter ()
        {
            _handlers = new List<SectionHandler>();
            _messages = new Dictionary<string, ConfigSection>();
        }

        public T GetMessage<T> () where T : ConfigSection, new()
        {
            var name = new T().Name;
            if(_messages.ContainsKey(name)) {
                var msg = _messages[name] as T;
                return msg;
            }
            return null;
        }

        public void AddHandler<T> (Action<T> handler) where T : ConfigSection, new()
        {
            var shc = new SectionHandler<T> { Handler = handler };

            _handlers.Add (shc);

            var msg = this.GetMessage<T> ();
            if (msg!=null) {
                shc.Call(msg);               
            }           
        }

        public void CallHandlers<T> (T msg) where T : ConfigSection
        {
            _messages[msg.Name] = msg;

            foreach (var shb in _handlers) 
            {
                var shc = shb as SectionHandler<T>;
                if(shc!=null) {
                    shc.Call(msg);                   
                }
            }
        }

        public bool AllCalled()
        {
            foreach (var shb in _handlers) 
            {
                if(shb.Count==0) {
                    return false;
                }
            }
            return true;
        }

    }
}

