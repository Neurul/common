using System;
using System.Collections.Generic;
using System.Text;

namespace org.neurul.Common
{
    public static class IOHelper
    {
        public static bool IsPathValidRootedLocal(String pathString)
        {
            Boolean isValidUri = Uri.TryCreate(pathString, UriKind.Absolute, out Uri pathUri);
            return isValidUri && pathUri != null && pathUri.IsLoopback;
        }
    }
}
