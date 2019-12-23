using System;
using System.Collections.Generic;
using System.Text;

namespace org.neurul.Common.Constants
{
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

            public struct TotalCount
            {
                public const string Key = "X-total-count";
            }
        }
    }
}
