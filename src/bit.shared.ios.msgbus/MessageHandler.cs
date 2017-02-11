using System;

namespace bit.shared.ios.msgbus
{
    public delegate void MessageHandler<T>(T msg);
}
