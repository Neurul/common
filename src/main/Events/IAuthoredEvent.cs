using CQRSlite.Events;
using System;

namespace org.neurul.Common.Events
{
    public interface IAuthoredEvent : IEvent
    {
        Guid AuthorId { get; set; }
    }
}
