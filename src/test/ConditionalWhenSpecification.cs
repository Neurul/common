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

using CQRSlite.Commands;
using CQRSlite.Domain;
using CQRSlite.Domain.Exception;
using CQRSlite.Events;
using CQRSlite.Snapshotting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Common.Test
{
    public abstract class ConditionalWhenSpecification<TAggregate, THandler, TCommand>
        where TAggregate : AggregateRoot
        where THandler : class
        where TCommand : ICommand
    {
        protected TAggregate Aggregate { get; set; }
        protected ISession Session { get; set; }
        protected abstract IEnumerable<IEvent> Given();
        protected abstract TCommand When();
        protected abstract THandler BuildHandler();
        protected dynamic handler;
        protected virtual bool InvokeWhenOnConstruct => true;
        protected virtual bool InvokeBuildWhenOnConstruct => true;

        protected Snapshot Snapshot { get; set; }
        protected IList<IEvent> EventDescriptors { get; set; }
        protected IList<IEvent> PublishedEvents { get; set; }

        public ConditionalWhenSpecification()
        {
            var eventpublisher = new SpecEventPublisher();
            var eventstorage = new SpecEventStorage(eventpublisher, Given().ToList());
            var snapshotstorage = new SpecSnapShotStorage(Snapshot);

            var snapshotStrategy = new DefaultSnapshotStrategy();
            var repository = new SnapshotRepository(snapshotstorage, snapshotStrategy, new Repository(eventstorage), eventstorage);
            Session = new Session(repository);
            Aggregate = GetAggregate().Result;

            if (this.InvokeBuildWhenOnConstruct)
                Task.Run(() => this.InvokeBuildWhen()).Wait();

            Snapshot = snapshotstorage.Snapshot;
            PublishedEvents = eventpublisher.PublishedEvents;
            EventDescriptors = eventstorage.Events;
        }

        protected async Task InvokeBuildWhen()
        {
            this.handler = BuildHandler();
            if (this.InvokeWhenOnConstruct)
                await InvokeWhen();
        }

        protected async Task InvokeWhen()
        {
            if (this.handler is ICancellableCommandHandler<TCommand>)
            {
                await this.handler.Handle(When(), new CancellationToken());
            }
            else if (this.handler is ICommandHandler<TCommand>)
            {
                await this.handler.Handle(When());
            }
            else
            {
                throw new InvalidCastException($"{nameof(this.handler)} is not a command handler of type {typeof(TCommand)}");
            }
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
}
