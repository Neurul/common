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

using Newtonsoft.Json;
using org.neurul.Common.Domain.Model;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace org.neurul.Common.Events
{
    public class NotificationLog
    {
        public NotificationLog(NotificationLogId notificationLogId, NotificationLogId firstNotificationLogId, NotificationLogId nextNotificationLogId, 
            NotificationLogId previousNotificationLogId, IEnumerable<Notification> notificationList, bool isArchived, int totalCount)
        {
            AssertionConcern.AssertArgumentNotNull(notificationLogId, nameof(notificationLogId));

            this.notificationLogId = notificationLogId;
            this.firstNotificationLogId = firstNotificationLogId;
            this.nextNotificationLogId = nextNotificationLogId;
            this.previousNotificationLogId = previousNotificationLogId;
            this.notificationList = new ReadOnlyCollection<Notification>(notificationList.ToArray());
            this.isArchived = isArchived;
            this.TotalCount = totalCount;
        }

        private readonly NotificationLogId notificationLogId;
        private readonly NotificationLogId firstNotificationLogId;
        private readonly NotificationLogId nextNotificationLogId;
        private readonly NotificationLogId previousNotificationLogId;
        private readonly ReadOnlyCollection<Notification> notificationList;
        private readonly bool isArchived;

        public bool IsArchived
        {
            get { return this.isArchived; }
        }

        public ReadOnlyCollection<Notification> NotificationList
        {
            get { return this.notificationList; }
        }

        public int TotalNotification
        {
            get { return this.notificationList.Count; }
        }

        public int TotalCount { get; private set; }

        [JsonIgnore]
        public NotificationLogId DecodedNotificationLogId
        {
            get { return this.notificationLogId; }
        }

        public string NotificationLogId
        {
            get { return this.notificationLogId?.Encoded; }
        }

        [JsonIgnore]
        public NotificationLogId DecodedNextNotificationLogId
        {
            get { return this.nextNotificationLogId; }
        }

        public string NextNotificationLogId
        {
            get { return this.nextNotificationLogId?.Encoded; }
        }

        public bool HasNextNotificationLog
        {
            get { return this.nextNotificationLogId != null; }
        }

        [JsonIgnore]
        public NotificationLogId DecodedPreviousNotificationLogId
        {
            get { return this.previousNotificationLogId; }
        }

        public string PreviousNotificationLogId
        {
            get { return this.previousNotificationLogId?.Encoded; }
        }

        public bool HasPreviousNotificationLog
        {
            get { return this.previousNotificationLogId != null; }
        }

        [JsonIgnore]
        public NotificationLogId DecodedFirstNotificationLogId
        {
            get { return this.firstNotificationLogId; }
        }

        public string FirstNotificationLogId
        {
            get { return this.firstNotificationLogId?.Encoded; }
        }

        public bool HasFirstNotificationLog
        {
            get { return this.firstNotificationLogId != null; }
        }
    }
}
