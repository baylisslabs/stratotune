using System;
using System.Collections.Generic;

using bit.shared.logging;

namespace bit.shared.ios.msgbus
{
    //
    // Currently implemented for single main-thread use
    //
    public class MsgBus : IMsgBus
    {
        static Logger _log = LogManager.GetLogger("MsgBus");
               
        private readonly string _busName;
        private Dictionary<string,IChannel> _channels;

        public MsgBus (string name)
        {
            _busName = name;
            _channels = new Dictionary<string, IChannel>();
        }

        public IChannel<T> CreateChannel<T> (ChannelOptions channelOptions) where T : IMessage, new()
        {
            var id = new T ().Id;           
            var channel = getChannel<T> (id);
            if (channel!=null) {
                _log.Warn("MsgBus '{0}': Channel '{1}' already created",_busName,id);
                return channel;
            }

            channel = channelOptions.CreateChannel<T>(_busName,id);                
            _channels.Add(id,channel);
            return channel;
        }

        public void Publish<T> (T msg) where T : IMessage, new()
        {
            var channel = getChannel<T> (msg.Id);
            if (channel==null) {
                _log.Warn("MsgBus '{0}': Channel '{1}' not found",_busName,msg.Id);
#if DEBUG
                throw new InvalidChannelException();
#else
                return;
#endif
            }

            channel.Publish(msg);
        }

        public void Publish<T> (T msg, MessageOptions opts) where T : IMessage, new()
        {
            var channel = getChannel<T> (msg.Id);
            if (channel==null) {
                _log.Warn("MsgBus '{0}': Channel '{1}' not found",_busName,msg.Id);
#if DEBUG
                throw new InvalidChannelException();
#else
                return;
#endif
            }
            
            channel.Publish(msg,opts);
        }

        public ISubscription Subscribe<T>(MessageHandler<T> handler) where T : IMessage, new()
        {
            var id = new T ().Id;
            var channel = getChannel<T> (id);
            if (channel==null) {
                _log.Warn("MsgBus '{0}': Channel '{1}' not found",_busName,id);
#if DEBUG
                throw new InvalidChannelException();
#else
                return null;
#endif
            }

            return channel.Subscribe(handler);
        }


        private IChannel<T> getChannel<T>(string id) where T : IMessage, new()
        {
            IChannel baseRef;
            if(_channels.TryGetValue(id,out baseRef)) {
                var channel = baseRef as IChannel<T>;
                return channel;
            }
            return null;
        }
    }
}

