using System;
using System.Collections.Generic;
using System.Text;

namespace neurUL.Common.Http
{
    public class Helper
    {
        public static string UrlCombine(string baseUrl, string path)
        {
            string result = string.Empty;
            if (Uri.TryCreate(new Uri(baseUrl), path, out Uri storeUrl))
                result = storeUrl.ToString();
            return result;
        }
    }
}
