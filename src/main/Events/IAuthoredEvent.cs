using CQRSlite.Events;

namespace org.neurul.Common.Events
{
    public interface IAuthoredEvent : IEvent
    {
        string AuthorId { get; set; }
    }
}
