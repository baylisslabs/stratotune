using System;

namespace bit.shared.ios.msgbus
{
    public struct ChannelOptions
    {      
        public enum QueueTypeOptions
        {
            Queue = 0,
            Store
        }

        // public enum PropagationOptions

        public QueueTypeOptions QueueType  { get; set; }
        // public int QueueMaxLength { get; set; }
    }

    public static class ChannelOptionsExtensions
    {
        internal static IChannel<T> CreateChannel<T> (this ChannelOptions opts, string busName, string id) where T : IMessage, new()
        {
            if (opts.QueueType == ChannelOptions.QueueTypeOptions.Queue) {
                return new QueuedTypeChannel<T> (busName, id, opts);
            } else if (opts.QueueType == ChannelOptions.QueueTypeOptions.Store) {
                return new StoredTypeChannel<T>(busName, id, opts);
            }
            throw new ArgumentException("unknown QueueType");
        }
    }
}

