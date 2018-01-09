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
    public class EventInfoLog
    {
        public EventInfoLog(EventInfoLogId eventInfoLogId, EventInfoLogId firstEventInfoLogId, EventInfoLogId nextEventInfoLogId, 
            EventInfoLogId previousEventInfoLogId, IEnumerable<EventInfo> eventInfoList, bool isArchived)
        {
            AssertionConcern.AssertArgumentNotNull(eventInfoLogId, nameof(eventInfoLogId));

            this.eventInfoLogId = eventInfoLogId;
            this.firstEventInfoLogId = firstEventInfoLogId;
            this.nextEventInfoLogId = nextEventInfoLogId;
            this.previousEventInfoLogId = previousEventInfoLogId;
            this.eventInfoList = new ReadOnlyCollection<EventInfo>(eventInfoList.ToArray());
            this.isArchived = isArchived;
        }

        private readonly EventInfoLogId eventInfoLogId;
        private readonly EventInfoLogId firstEventInfoLogId;
        private readonly EventInfoLogId nextEventInfoLogId;
        private readonly EventInfoLogId previousEventInfoLogId;
        private readonly ReadOnlyCollection<EventInfo> eventInfoList;
        private readonly bool isArchived;

        public bool IsArchived
        {
            get { return this.isArchived; }
        }

        public ReadOnlyCollection<EventInfo> EventInfoList
        {
            get { return this.eventInfoList; }
        }

        public int TotalEventInfo
        {
            get { return this.eventInfoList.Count; }
        }

        [JsonIgnore]
        public EventInfoLogId DecodedEventInfoLogId
        {
            get { return this.eventInfoLogId; }
        }

        public string EventInfoLogId
        {
            get { return this.eventInfoLogId?.Encoded; }
        }

        [JsonIgnore]
        public EventInfoLogId DecodedNextEventInfoLogId
        {
            get { return this.nextEventInfoLogId; }
        }

        public string NextEventInfoLogId
        {
            get { return this.nextEventInfoLogId?.Encoded; }
        }

        public bool HasNextEventInfoLog
        {
            get { return this.nextEventInfoLogId != null; }
        }

        [JsonIgnore]
        public EventInfoLogId DecodedPreviousEventInfoLogId
        {
            get { return this.previousEventInfoLogId; }
        }

        public string PreviousEventInfoLogId
        {
            get { return this.previousEventInfoLogId?.Encoded; }
        }

        public bool HasPreviousEventInfoLog
        {
            get { return this.previousEventInfoLogId != null; }
        }

        [JsonIgnore]
        public EventInfoLogId DecodedFirstEventInfoLogId
        {
            get { return this.firstEventInfoLogId; }
        }

        public string FirstEventInfoLogId
        {
            get { return this.firstEventInfoLogId?.Encoded; }
        }

        public bool HasFirstEventInfoLog
        {
            get { return this.firstEventInfoLogId != null; }
        }
    }
}
