// Copyright 2012,2013 Vaughn Vernon
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// 
// Modifications copyright(C) 2017 ei8.works/Elmer Bool

using org.neurul.Common.Domain.Model;
using System.Threading.Tasks;

namespace org.neurul.Common.Events
{
    public class EventInfoLogFactory
    {
        public const int EVENTS_PER_LOG = 20;

        public EventInfoLogFactory(INavigableEventStore eventStore)
        {
            this.eventStore = eventStore;
        }

        readonly INavigableEventStore eventStore;

        public async Task<EventInfoLog> CreateEventInfoLog(EventInfoLogId eventInfoLogId)
        {
            AssertionConcern.AssertArgumentValid<long>(
                l => (eventInfoLogId.High % EVENTS_PER_LOG) == 0, 
                eventInfoLogId.High, 
                $"LogId 'High' value must be divisible by '{EVENTS_PER_LOG}'", 
                nameof(eventInfoLogId)
                );
            AssertionConcern.AssertArgumentValid<long>(
                l =>  (eventInfoLogId.Low - 1 == 0) || ((eventInfoLogId.Low - 1) % EVENTS_PER_LOG) == 0,
                eventInfoLogId.Low,
                $"LogId 'Low' value must be equal to 1 or, 1 plus a number divisible by '{EVENTS_PER_LOG}'",
                nameof(eventInfoLogId)
                );

            var count = await this.eventStore.CountEventInfo();
            return await this.CreateEventInfoLog(new EventInfoLogInfo(eventInfoLogId, count));
        }

        public async Task<EventInfoLog> CreateCurrentEventInfoLog()
        { 
            return await this.CreateEventInfoLog(await this.CalculateCurrentEventInfoLogId());
        }

        private async Task<EventInfoLogInfo> CalculateCurrentEventInfoLogId()
        {
            var count = await this.eventStore.CountEventInfo();
            long low, high;
            if (count > 0)
            {
                var remainder = count % EVENTS_PER_LOG;
                if (remainder == 0)
                {
                    remainder = EVENTS_PER_LOG;
                }
                low = count - remainder + 1;
                high = low + EVENTS_PER_LOG - 1;                
            }
            else
                low = high = 0;

            return new EventInfoLogInfo(new EventInfoLogId(low, high), count);
        }

        private async Task<EventInfoLog> CreateEventInfoLog(EventInfoLogInfo eventInfoLogInfo)
        {
            var eventInfoList = new EventInfo[0];
            EventInfoLogId first = null, next = null, previous = null;
            var isArchived = false;
            if (eventInfoLogInfo.TotalLogged > 0)
            {
                eventInfoList = await this.eventStore.GetEventInfoRange(eventInfoLogInfo.EventInfoLogId.Low, eventInfoLogInfo.EventInfoLogId.High);
                first = eventInfoLogInfo.EventInfoLogId.First(EVENTS_PER_LOG, eventInfoLogInfo.TotalLogged);
                next = eventInfoLogInfo.EventInfoLogId.Next(EVENTS_PER_LOG, eventInfoLogInfo.TotalLogged);
                previous = eventInfoLogInfo.EventInfoLogId.Previous(EVENTS_PER_LOG, eventInfoLogInfo.TotalLogged);
                var currentLog = await this.CalculateCurrentEventInfoLogId();
                isArchived =
                    eventInfoLogInfo.EventInfoLogId.High < currentLog.EventInfoLogId.Low ||
                    (
                        eventInfoLogInfo.TotalLogged >= EVENTS_PER_LOG &&
                        eventInfoLogInfo.TotalLogged == eventInfoLogInfo.EventInfoLogId.High &&
                        eventInfoLogInfo.EventInfoLogId.High == currentLog.EventInfoLogId.High
                    );
            }

            return new EventInfoLog(
                new EventInfoLogId(eventInfoLogInfo.EventInfoLogId),
                first,
                next,
                previous,
                eventInfoList,
                isArchived
                );                
        }
    }
}
