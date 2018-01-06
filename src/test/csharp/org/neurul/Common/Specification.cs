//
//Copyright 2017 Gaute Magnussen
//
//Licensed under the Apache License, Version 2.0 (the "License"); you may not use this 
//file except in compliance with the License. You may obtain a copy of the License at
//
//http://www.apache.org/licenses/LICENSE-2.0
//
//Unless required by applicable law or agreed to in writing, software distributed under 
//the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY 
//KIND, either express or implied. See the License for the specific language governing 
//permissions and limitations under the License.
//
// Modifications copyright(C) 2018 ei8.works/Elmer Bool

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CQRSlite.Commands;
using CQRSlite.Domain;
using CQRSlite.Domain.Exception;
using CQRSlite.Events;
using CQRSlite.Snapshotting;

namespace Common.Test
{
    public abstract class Specification<TAggregate, THandler, TCommand> 
        where TAggregate: AggregateRoot
        where THandler : class
        where TCommand : ICommand
    {

        protected TAggregate Aggregate { get; set; }
        protected ISession Session { get; set; }
        protected abstract IEnumerable<IEvent> Given();
        protected abstract TCommand When();
        protected abstract THandler BuildHandler();

        protected Snapshot Snapshot { get; set; }
        protected IList<IEvent> EventDescriptors { get; set; }
        protected IList<IEvent> PublishedEvents { get; set; }

        public Specification()
        {
            var eventpublisher = new SpecEventPublisher();
            var eventstorage = new SpecEventStorage(eventpublisher, Given().ToList());
            var snapshotstorage = new SpecSnapShotStorage(Snapshot);

            var snapshotStrategy = new DefaultSnapshotStrategy();
            var repository = new SnapshotRepository(snapshotstorage, snapshotStrategy, new Repository(eventstorage), eventstorage);
            Session = new Session(repository);
            Aggregate = GetAggregate().Result;

            dynamic handler = BuildHandler();
            if (handler is ICancellableCommandHandler<TCommand>)
            {
                handler.Handle(When(), new CancellationToken());
            }
            else if(handler is ICommandHandler<TCommand>)
            {
                handler.Handle(When());
            }
            else
            {
                throw new InvalidCastException($"{nameof(handler)} is not a command handler of type {typeof(TCommand)}");
            }

            Snapshot = snapshotstorage.Snapshot;
            PublishedEvents = eventpublisher.PublishedEvents;
            EventDescriptors = eventstorage.Events;
        }

        private async Task<TAggregate> GetAggregate()
        {
            try
            {
                return await Session.Get<TAggregate>(Guid.Empty);
            }
            catch (AggregateNotFoundException)
            {
                return null;
            }
        }
    }

    internal class SpecSnapShotStorage : ISnapshotStore
    {
        public SpecSnapShotStorage(Snapshot snapshot)
        {
            Snapshot = snapshot;
        }

        public Snapshot Snapshot { get; set; }

        public Task<Snapshot> Get(Guid id, CancellationToken cancellationToken = default(CancellationToken))
        {
            return Task.FromResult(Snapshot);
        }

        public Task Save(Snapshot snapshot, CancellationToken cancellationToken = default(CancellationToken))
        {
            Snapshot = snapshot;
            return Task.CompletedTask;
        }
    }

    internal class SpecEventPublisher : IEventPublisher
    {
        public SpecEventPublisher()
        {
            PublishedEvents = new List<IEvent>();
        }

        public Task Publish<T>(T @event, CancellationToken cancellationToken = default(CancellationToken)) where T : class, IEvent
        {
            PublishedEvents.Add(@event);
            return Task.CompletedTask;
        }

        public IList<IEvent> PublishedEvents { get; set; }
    }

    internal class SpecEventStorage : IEventStore
    {
        private readonly IEventPublisher _publisher;

        public SpecEventStorage(IEventPublisher publisher, List<IEvent> events)
        {
            _publisher = publisher;
            Events = events;
        }

        public List<IEvent> Events { get; set; }

        public Task Save(IEnumerable<IEvent> events, CancellationToken cancellationToken = default(CancellationToken))
        {
            Events.AddRange(events);
            return Task.WhenAll(events.Select(evt =>_publisher.Publish(evt, cancellationToken)));
                
        }

        public Task<IEnumerable<IEvent>> Get(Guid aggregateId, int fromVersion, CancellationToken cancellationToken = default(CancellationToken))
        {
            var events = Events.Where(x => x.Id == aggregateId && x.Version > fromVersion);
            return Task.FromResult(events);
        }
    }
}
