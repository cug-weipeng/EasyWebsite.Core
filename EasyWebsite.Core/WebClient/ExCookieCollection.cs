using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace EasyWebsite.Core.WebClient
{
    public class ExCookieCollection : CookieCollection
    {
        public const string ASPXAUTH = ".ASPXAUTH";
        public const string SSID = ".SSID";

        public Cookie GetCookie(string cookieName)
        {
            foreach(Cookie cookie in this)
            {
                if(cookie.Name.ToUpper() == cookieName.ToUpper())
                {
                    return cookie;
                }
            }
            return null;
        }

        public List<Cookie> GetCookies(string cookieName)
        {
            List<Cookie> cookies = new List<Cookie>();
            foreach (Cookie cookie in this)
            {
                if (cookie.Name.ToUpper() == cookieName.ToUpper())
                {
                    cookies.Add(cookie);
                }
            }
            return cookies;
        }

        public string GetCookieValue(string cookieName)
        {
            return GetCookie(cookieName).Value;
        }

        public bool Contains(string cookieName)
        {
            if (GetCookie(cookieName) != null)
                return true;
            return false;
        }

        public bool IsAspAuthenticate { 
            get
            {
                if (Contains(ASPXAUTH))
                    return true;
                return false;
            }
        }
    }
}
