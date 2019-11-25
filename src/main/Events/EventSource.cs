using CQRSlite.Domain;
using CQRSlite.Events;
using System;
using System.Collections.Generic;
using System.Text;

namespace org.neurul.Common.Events
{
    public class EventSource
    {
        public EventSource(string storeId, ISession session, IRepository repository, IEventStore eventStoreClient, INotificationClient notificationClient)
        {
            this.StoreId = storeId;
            this.Session = session;
            this.Repository = repository;
            this.EventStoreClient = eventStoreClient;
            this.NotificationClient = notificationClient;
        }

        public string StoreId { get; private set; }

        public ISession Session { get; private set; }

        public IRepository Repository { get; private set; }

        public IEventStore EventStoreClient { get; private set; }

        public INotificationClient NotificationClient { get; private set; }
    }
}
