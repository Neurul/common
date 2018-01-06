using System;
using System.Collections.Generic;
using System.Text;

namespace org.neurul.Common.Constants
{
    public struct Event
    {
        public struct EventInfoLog
        {
            public struct LogId
            {
                public struct Regex
                {
                    public const string Pattern = @"^
(?<Low>[\d]+)
\x2C
(?<High>[\d]+)
\z";
                    public struct CaptureName
                    {
                        public const string Low = "Low";
                        public const string High = "High";
                    }
                }
            }
        }

        public struct EventInfo
        {
            public struct TypeName
            {
                public struct Regex
                {
                    public const string Pattern = @"^
org
\x2E
neurul
\x2E
Brain
\x2E
Domain
\x2E
Model
\x2E
Neurons
\x2E
(?<EventName>[^\x2C]+)
\x2C
.*
\z";
                    public struct CaptureName
                    {
                        public const string EventName = "EventName";
                    }
                }
            }
        }
    }

    public struct Response
    {
        public struct Header
        {
            public struct Link
            {
                public const string Key = "Link";

                public struct Regex
                {
                    public const string Pattern = @"\x3C
(?<Uri>[^\x3E]+)
\x3E
\x3B
\s
rel
\x3D
\x22
(?<Relation>[^\x22]+)
\x22
\x2C?
\s?
";
                    public struct CaptureName
                    {
                        public const string Uri = "Uri";
                        public const string Relation = "Relation";
                    }
                }

                public enum Relation
                {
                    First,
                    Last,
                    Next,
                    Previous,
                    Self
                }
            }
        }
    }
}
