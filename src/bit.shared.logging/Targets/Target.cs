using System;

namespace bit.shared.logging
{
    public interface Target
    {
        void Write(LogEventInfo info);
    }
}

