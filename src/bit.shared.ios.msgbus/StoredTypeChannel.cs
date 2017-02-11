using System;
using System.Collections.Generic;

using MonoTouch.CoreFoundation;

using bit.shared.logging;

namespace bit.shared.ios.msgbus
{
    public class StoredTypeChannel<T> : ChannelBase<T>  where T : IMessage
    {
        private T _storedMsg;
        int _nextSeq;

        internal StoredTypeChannel (string busName, string id, ChannelOptions channelOptions) 
            : base(busName,id,channelOptions)
        {
        }

        public override void Publish (T msg)
        {                     
            msg.Seq = _nextSeq++;
            _storedMsg = msg;
            scheduleDeliver(this.Subscribers.First);  
        }

        public override void Publish (T msg, MessageOptions opts)
        {     
            this.Publish(msg);          
        }

        protected override void PrepareSubscription (LinkedListNode<MessageHandler<T>> subscriber)
        {
            scheduleDeliver(subscriber);
        }

        private void scheduleDeliver(LinkedListNode<MessageHandler<T>> firstSubNode)
        {
            if (!this.DeliveryPending) {
                this.DeliveryPending = true;
                this.GCDQueue.DispatchAsync (()=>{deliver(firstSubNode);});
            }  
        }

        private void deliver(LinkedListNode<MessageHandler<T>> firstSubNode)
        {
            if(_storedMsg!=null) {
                this.DeliverSingleMsg(_storedMsg,firstSubNode);
            }            
            this.DeliveryPending = false;
        }
    }
    
}
