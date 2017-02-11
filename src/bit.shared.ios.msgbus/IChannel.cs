using System;

namespace bit.shared.ios.msgbus
{   
    public interface IChannel
    {
    }

    public interface IChannel<T> : IChannel
    {
        void Publish (T msg);
        void Publish (T msg, MessageOptions opts);
        ISubscription Subscribe(MessageHandler<T> handler);       
    }
    
}
