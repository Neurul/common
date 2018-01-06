using CQRSlite.Events;
using System;
using System.Collections.Generic;
using System.Text;

namespace org.neurul.Common.Events
{
    public interface IEventSerializer
    {
        IEvent Deserialize(string typeName, string eventData);

        string Serialize(IEvent @event);
    }
}
