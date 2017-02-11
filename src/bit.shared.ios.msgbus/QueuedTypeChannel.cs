using System;
using System.Collections.Generic;

using MonoTouch.CoreFoundation;
using MonoTouch.Foundation;

using bit.shared.logging;

namespace bit.shared.ios.msgbus
{

    public class QueuedTypeChannel<T> : ChannelBase<T>  where T : IMessage
    {
        private LinkedList<T> _msgQueue;
        private int _nextSeq;

        internal QueuedTypeChannel (string busName, string id, ChannelOptions channelOptions) 
            : base(busName,id,channelOptions)
        {
            _msgQueue = new LinkedList<T>();
        }

        public override void Publish (T msg)
        {
            msg.Seq = _nextSeq++;
            _msgQueue.AddLast (msg);
            scheduleDeliver (this.Subscribers.First); 
        }

        public override void Publish (T msg, MessageOptions opts)
        {                  
            if (opts.DelaySeconds == 0) {
                this.Publish(msg);
            } else {
                msg.Seq = _nextSeq++;
                scheduleDeliverWithDelay (this.Subscribers.First, msg, opts.DelaySeconds); 
            }
        }

        private void scheduleDeliverWithDelay(LinkedListNode<MessageHandler<T>> firstSubNode, T msg, double delaySeconds)
        {
            // nb. at the moment this assumes single threaded msgbus operation, and will execute on the main run-loop
            var timer = NSTimer.CreateTimer(delaySeconds,()=>{this.DeliverSingleMsg(msg,firstSubNode);});
            NSRunLoop.Main.AddTimer(timer,NSRunLoopMode.Common);
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
            var msgNode = _msgQueue.First;
            while (msgNode!=null) {
                this.DeliverSingleMsg(msgNode.Value,firstSubNode);
                _msgQueue.Remove(msgNode);
                msgNode = _msgQueue.First;
            }
            
            this.DeliveryPending = false;
        }

    }
    
}
