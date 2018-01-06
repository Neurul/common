using CQRSlite.Events;
using org.neurul.Common.Constants;
using System;
using System.Text.RegularExpressions;

namespace org.neurul.Common.Events
{
    public static class EventExtensions
    {
        public static EventInfo ToEventInfo(this IEvent @event, IEventSerializer serializer)
        {
            var contentJson = serializer.Serialize(@event);

            if (string.IsNullOrEmpty(contentJson))
                throw new InvalidOperationException("Failed deserializing event.");

            return new EventInfo()
            {
                Id = @event.Id.ToString(),
                Data = contentJson,
                TypeName = @event.GetType().AssemblyQualifiedName,
                Timestamp = DateTimeOffset.Now.ToString("o"),
                Version = @event.Version
            };
        }

        
        public static string GetEventName(this EventInfo @event)
        {
            var m = Regex.Match(
                @event.TypeName,
                Common.Constants.Event.EventInfo.TypeName.Regex.Pattern,
                RegexOptions.Multiline | RegexOptions.IgnorePatternWhitespace
                );

            return m.Success ? m.Groups[Event.EventInfo.TypeName.Regex.CaptureName.EventName].Value : null;
        }

        public static IEvent ToDomainEvent(this EventInfo @event, IEventSerializer serializer)
        {
            return ToDomainEvent<IEvent>(@event, serializer);
        }

        public static TEvent ToDomainEvent<TEvent>(this EventInfo @event, IEventSerializer serializer)
            where TEvent : IEvent
        {
            return (TEvent)serializer.Deserialize(@event.TypeName, @event.Data);
        }
    }
}
