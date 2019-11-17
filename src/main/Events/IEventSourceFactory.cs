using System;
using System.Collections.Generic;
using System.Text;

namespace org.neurul.Common.Events
{
    public interface IEventSourceFactory
    {
        EventSource CreateEventSource(string storeId);
    }
}
