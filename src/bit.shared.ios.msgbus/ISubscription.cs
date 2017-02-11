using System;

using bit.shared.logging;
using System.Collections.Generic;

namespace bit.shared.ios.msgbus
{
    public interface ISubscription
    {
        void Unsubscribe ();
    };      
}
