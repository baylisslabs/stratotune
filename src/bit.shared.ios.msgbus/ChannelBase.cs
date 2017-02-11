using System;
using System.Collections.Generic;

using MonoTouch.CoreFoundation;
using MonoTouch.Foundation;

using bit.shared.logging;

namespace bit.shared.ios.msgbus
{
    public abstract class ChannelBase<T> : IChannel<T> where T : IMessage
    {       
        static Logger _log = LogManager.GetLogger("Channel");

        protected string BusName { get; set; }
        protected string Id { get; set; }
        protected ChannelOptions Options { get; set; }
        protected LinkedList<MessageHandler<T>>  Subscribers { get; set; }           
        protected DispatchQueue GCDQueue { get; set; }
        protected bool DeliveryPending { get; set; }
              
        internal ChannelBase (string busName, string id, ChannelOptions channelOptions)
        {
            _log.Debug ("MsgBus '{0}': CreateChannel '{1}'", busName, id);
            this.BusName = busName;
            this.Id = id;          
            this.Options = channelOptions;
            this.Subscribers = new LinkedList<MessageHandler<T>>();
        
            this.GCDQueue = DispatchQueue.MainQueue;
        }

        public abstract void Publish (T msg);      
        public abstract void Publish (T msg, MessageOptions opts);       
        
        public ISubscription Subscribe(MessageHandler<T> handler)
        {           
            var node = this.Subscribers.AddLast(handler);   
            this.PrepareSubscription(node);
            return new SubscriptionHandle<T> { Subscribers = this.Subscribers, SubscriberNode = node };
        }

        protected virtual void PrepareSubscription(LinkedListNode<MessageHandler<T>> subscriber)
        {    
        }
                      
        protected void DeliverSingleMsg (T msg, LinkedListNode<MessageHandler<T>> firstSubscriber)
        {         
            var subscriber = firstSubscriber;
            while (subscriber!=null) {
                try {
                    subscriber.Value (msg);
                } catch (Exception ex) {
                    _log.Warn (String.Format ("MsgBus '{0}': Channel '{1}' handler threw exception", this.BusName, this.Id), ex);
#if DEBUG
                    throw;
#endif                   
                }
                subscriber = subscriber.Next;
            }              
        }
    }
}

