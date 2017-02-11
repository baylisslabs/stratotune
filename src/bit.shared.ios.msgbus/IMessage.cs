using System;

namespace bit.shared.ios.msgbus
{
    public interface IMessage
    {
        string Id { get; }
        int Seq { get; set; }
    }
}

