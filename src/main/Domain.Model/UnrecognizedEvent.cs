using CQRSlite.Events;
using System;
using System.Collections.Generic;
using System.Text;

namespace org.neurul.Common.Domain.Model
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
