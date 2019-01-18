using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace EasyWebsite.Core.WebClient
{
    public static class CookieExtension
    {
        private static readonly Regex _CookieRegex = new Regex(@"^(.+?)\s*=\s*(.+?)\b");

        public static ExCookieCollection GetCookieCollection(this HttpResponseHeaders header)
        {
            if (header.Contains("set-Cookie"))
            {
                ExCookieCollection collection = new ExCookieCollection();
                header.GetValues("set-Cookie").ToList().ForEach(t => collection.Add(GetCookiesFromString(t)));
                return collection;
            }
            return null;
        }

        private static ExCookieCollection GetCookiesFromString(string set_cookies)
        {
            if (string.IsNullOrWhiteSpace(set_cookies))
                return null;

            ExCookieCollection collection = new ExCookieCollection();
            foreach (var setcookie in set_cookies.Split(','))
            {
                var cookie = GetCookieFromString(setcookie);
                if (cookie != null)
                    collection.Add(cookie);
            }

            return collection;
        }

        private static Cookie GetCookieFromString(string set_cookie)
        {
            var match = _CookieRegex.Match(set_cookie);
            if (match.Groups.Count > 0)
            {
                return new Cookie(match.Groups[1].Value, match.Groups[2].Value);
            }
            return null;
        }
    }
}
