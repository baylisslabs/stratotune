using System;
using System.Collections.Generic;

using bit.shared.logging;

namespace bit.shared.ios.msgbus
{
    public interface IMsgBus
    {
        IChannel<T> CreateChannel<T> (ChannelOptions channelOptions) where T : IMessage, new();       
        void Publish<T> (T msg) where T : IMessage, new();      
        void Publish<T> (T msg, MessageOptions opts) where T : IMessage, new();      
        ISubscription Subscribe<T>(MessageHandler<T> handler) where T : IMessage, new();
    }  
}
