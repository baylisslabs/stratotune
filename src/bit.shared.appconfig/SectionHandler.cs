using System;

namespace bit.shared.appconfig
{
    internal abstract class SectionHandler
    {       
        public int Count { get; set; }
    }

    internal class SectionHandler<T> : SectionHandler where T : ConfigSection
    {
        public Action<T> Handler { get; set; } 

        public void Call(T msg)
        {
            this.Handler(msg);
            this.Count++;
        }
    }
}

