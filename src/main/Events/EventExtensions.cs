using CQRSlite.Events;
using org.neurul.Common.Constants;
using System;
using System.Text.RegularExpressions;

namespace org.neurul.Common.Events
{
    public static class EventExtensions
    {
        public static Notification ToNotification(this IAuthoredEvent @event, IEventSerializer serializer)
        {
            var contentJson = serializer.Serialize(@event);

            if (string.IsNullOrEmpty(contentJson))
                throw new InvalidOperationException("Failed deserializing event.");

            return new Notification()
            {
                Id = @event.Id.ToString(),
                Data = contentJson,
                TypeName = @event.GetType().AssemblyQualifiedName,
                Timestamp = DateTimeOffset.Now.ToString("o"),
                Version = @event.Version,
                AuthorId = @event.AuthorId.ToString()
            };
        }

        
        public static string GetEventName(this Notification @event)
        {
            var m = Regex.Match(
                @event.TypeName,
                Common.Constants.Event.TypeName.Regex.Pattern,
                RegexOptions.Multiline | RegexOptions.IgnorePatternWhitespace
                );

            return m.Success ? m.Groups[Event.TypeName.Regex.CaptureName.EventName].Value : null;
        }

        public static IAuthoredEvent ToDomainEvent(this Notification @event, IEventSerializer serializer)
        {
            return ToDomainEvent<IAuthoredEvent>(@event, serializer);
        }

        public static TEvent ToDomainEvent<TEvent>(this Notification @event, IEventSerializer serializer)
            where TEvent : IAuthoredEvent
        {
            return (TEvent)serializer.Deserialize(@event.TypeName, @event.Data);
        }
    }
}
