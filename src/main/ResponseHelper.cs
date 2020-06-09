using neurUL.Common.Constants;
using neurUL.Common.Domain.Model;
using System.Text;
using System.Text.RegularExpressions;

namespace neurUL.Common
{
    public static class ResponseHelper
    {
        public static class Header
        {
            public static class Link
            {
                public static bool TryGet(string linkHeaderValue, Response.Header.Link.Relation relation, out string link)
                {
                    AssertionConcern.AssertArgumentNotNull(linkHeaderValue, nameof(linkHeaderValue));

                    bool result = false;
                    link = null;

                    var ms = Regex.Matches(linkHeaderValue, Response.Header.Link.Regex.Pattern, RegexOptions.Multiline | RegexOptions.IgnorePatternWhitespace);
                    foreach (Match m in ms)
                        if (m.Groups[Response.Header.Link.Regex.CaptureName.Relation].Value.ToUpper() == relation.ToString().ToUpper())
                        {
                            link = m.Groups[Response.Header.Link.Regex.CaptureName.Uri].Value;
                            result = true;
                        }

                    return result;
                }

                public static void AppendValue(StringBuilder builder, string uri, Response.Header.Link.Relation relation)
                {
                    if (builder.Length > 0)
                        builder.Append(", ");

                    builder.Append('<');
                    builder.Append(uri);
                    builder.Append('>');
                    builder.Append("; rel=\"");
                    builder.Append(relation.ToString().ToLower());
                    builder.Append('"');
                }
            }
        }
    }
}
