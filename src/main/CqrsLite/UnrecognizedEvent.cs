using CQRSlite.Events;
using System;

namespace neurUL.Common.CqrsLite
{
    public class UnrecognizedEvent : IEvent
    {
        public string TypeName { get; set; }
        public string Data { get; set; }
        public Guid Id { get; set; }
        public int Version { get; set; }
        public DateTimeOffset TimeStamp { get; set; }
    }
}
