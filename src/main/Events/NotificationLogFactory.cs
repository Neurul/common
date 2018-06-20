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
    public class NotificationLogFactory
    {
        public const int EVENTS_PER_LOG = 20;

        public NotificationLogFactory(INavigableEventStore eventStore)
        {
            this.eventStore = eventStore;
        }

        readonly INavigableEventStore eventStore;

        public async Task<NotificationLog> CreateNotificationLog(NotificationLogId notificationLogId)
        {
            AssertionConcern.AssertArgumentValid<long>(
                l => (notificationLogId.High % EVENTS_PER_LOG) == 0, 
                notificationLogId.High, 
                $"LogId 'High' value must be divisible by '{EVENTS_PER_LOG}'", 
                nameof(notificationLogId)
                );
            AssertionConcern.AssertArgumentValid<long>(
                l =>  (notificationLogId.Low - 1 == 0) || ((notificationLogId.Low - 1) % EVENTS_PER_LOG) == 0,
                notificationLogId.Low,
                $"LogId 'Low' value must be equal to 1 or, 1 plus a number divisible by '{EVENTS_PER_LOG}'",
                nameof(notificationLogId)
                );

            var count = await this.eventStore.CountNotifications();
            return await this.CreateNotificationLog(new NotificationLogInfo(notificationLogId, count));
        }

        public async Task<NotificationLog> CreateCurrentNotificationLog()
        { 
            return await this.CreateNotificationLog(await this.CalculateCurrentNotificationLogId());
        }

        private async Task<NotificationLogInfo> CalculateCurrentNotificationLogId()
        {
            var count = await this.eventStore.CountNotifications();
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

            return new NotificationLogInfo(new NotificationLogId(low, high), count);
        }

        private async Task<NotificationLog> CreateNotificationLog(NotificationLogInfo notificationLogInfo)
        {
            var notificationList = new Notification[0];
            NotificationLogId first = null, next = null, previous = null;
            var isArchived = false;
            if (notificationLogInfo.TotalLogged > 0)
            {
                notificationList = await this.eventStore.GetNotificationRange(notificationLogInfo.NotificationLogId.Low, notificationLogInfo.NotificationLogId.High);
                first = notificationLogInfo.NotificationLogId.First(EVENTS_PER_LOG, notificationLogInfo.TotalLogged);
                next = notificationLogInfo.NotificationLogId.Next(EVENTS_PER_LOG, notificationLogInfo.TotalLogged);
                previous = notificationLogInfo.NotificationLogId.Previous(EVENTS_PER_LOG, notificationLogInfo.TotalLogged);
                var currentLog = await this.CalculateCurrentNotificationLogId();
                isArchived =
                    notificationLogInfo.NotificationLogId.High < currentLog.NotificationLogId.Low ||
                    (
                        notificationLogInfo.TotalLogged >= EVENTS_PER_LOG &&
                        notificationLogInfo.TotalLogged == notificationLogInfo.NotificationLogId.High &&
                        notificationLogInfo.NotificationLogId.High == currentLog.NotificationLogId.High
                    );
            }

            return new NotificationLog(
                new NotificationLogId(notificationLogInfo.NotificationLogId),
                first,
                next,
                previous,
                notificationList,
                isArchived
                );                
        }
    }
}
