using System;
using System.Collections.Generic;

using MonoTouch.CoreFoundation;

using bit.shared.logging;

namespace bit.shared.ios.msgbus
{
    internal class SubscriptionHandle<T> : ISubscription where T : IMessage
    {
        static Logger _log = LogManager.GetLogger("SubscriptionHandle");
        public LinkedList<MessageHandler<T>> Subscribers { get; set; }
        public LinkedListNode<MessageHandler<T>> SubscriberNode { get; set; }
        public void Unsubscribe ()
        {
            if (Subscribers != null && SubscriberNode != null) {
                try {Subscribers.Remove(SubscriberNode); } catch(Exception ex) { _log.Warn("Unsubscribe failed",ex); }
            }
            Subscribers = null;
            SubscriberNode = null;
        }
    }
    
}
