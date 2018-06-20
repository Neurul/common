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

using SQLite;
using System;

namespace org.neurul.Common.Events
{
    public class Notification : IEquatable<Notification>
    {
        [PrimaryKey, AutoIncrement]
        public long SequenceId { get; set; }

        public string Timestamp { get; set; }

        public string TypeName { get; set; }

        public string Id { get; set; }

        public int Version { get; set; }

        public string AuthorId { get; set; }

        public string Data { get; set; }

        public bool Equals(Notification other)
        {
            if (object.ReferenceEquals(this, other)) return true;
            if (object.ReferenceEquals(null, other)) return false;
            return this.SequenceId.Equals(other.SequenceId);
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as Notification);
        }

        public override int GetHashCode()
        {
            return this.SequenceId.GetHashCode();
        }
    }
}
