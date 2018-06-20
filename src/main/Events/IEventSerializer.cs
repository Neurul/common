using CQRSlite.Events;
using System;
using System.Collections.Generic;
using System.Text;

namespace org.neurul.Common.Events
{
    public interface IEventSerializer
    {
        IAuthoredEvent Deserialize(string typeName, string eventData);

        string Serialize(IAuthoredEvent @event);
    }
}
