using CQRSlite.Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace org.neurul.Common.Events
{
    public class EventSource
    {
        public EventSource(string storeId, ISession session, IRepository repository, INavigableEventStore eventStore)
        {
            this.StoreId = storeId;
            this.Session = session;
            this.Repository = repository;
            this.EventStore = eventStore;
        }

        public string StoreId { get; private set; }

        public ISession Session { get; private set; }

        public IRepository Repository { get; private set; }

        public INavigableEventStore EventStore { get; private set; }
    }
}
