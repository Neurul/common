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

using org.neurul.Common.Constants;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace org.neurul.Common.Events
{
    public class NotificationLogId : Domain.Model.ValueObject
    {
        public static string GetEncoded(NotificationLogId notificationLogId)
        {
            if (notificationLogId != null) return notificationLogId.Encoded;
            else return null;
        }

        public NotificationLogId(long lowId, long highId)
        {
            this.Low = lowId;
            this.High = highId;
        }

        public NotificationLogId(NotificationLogId original)
        {
            this.Low = original.Low;
            this.High = original.High;
        }

        public static bool TryParse(string value, out NotificationLogId logId)
        {
            bool result = false;
            logId = null;

            var m = Regex.Match(value, Event.NotificationLog.LogId.Regex.Pattern, RegexOptions.Multiline | RegexOptions.IgnorePatternWhitespace);
            if (m.Success)
            {
                logId = new NotificationLogId(
                    long.Parse(m.Groups[Event.NotificationLog.LogId.Regex.CaptureName.Low].Value),
                    long.Parse(m.Groups[Event.NotificationLog.LogId.Regex.CaptureName.High].Value)
                    );
                result = true;
            }

            return result;
        }

        public long Low { get; private set; }
        public long High { get; private set; }

        public string Encoded
        {
            get { return this.Low + "," + this.High; }
        }

        public NotificationLogId First(int notificationsPerLog, long totalLogged)
        {
            var first = new NotificationLogId(1, notificationsPerLog);
            if (totalLogged < 1)
                first = null;
            return first;
        }

        public NotificationLogId Next(int notificationsPerLog, long totalLogged)
        {
            var nextLow = this.High + 1;
            var nextHigh = nextLow + notificationsPerLog - 1;
            var next = new NotificationLogId(nextLow, nextHigh);
            if (nextLow > totalLogged)
                next = null; 
            return next;
        }

        public NotificationLogId Previous(int notificationsPerLog, long totalLogged)
        {
            var previousLow = Math.Max(this.Low - notificationsPerLog, 1);
            var previousHigh = this.Low - 1;
            var previous = new NotificationLogId(previousLow, previousHigh);
            if (previousHigh <= 0 || previousLow > totalLogged)
                previous = null;
            return previous;
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return this.Low;
            yield return this.High;
        }
    }
}
