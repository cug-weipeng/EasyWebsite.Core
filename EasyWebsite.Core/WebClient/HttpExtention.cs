using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace EasyWebsite.Core.WebClient
{
    public static class HttpExtention
    {
        public static bool IsRedirectCode(this HttpResponseMessage response)
        {
            if (response.StatusCode == HttpStatusCode.Redirect
                   || response.StatusCode == HttpStatusCode.Found
                   || response.StatusCode == HttpStatusCode.SeeOther)
            {
                return true;
            }
            return false;
        }
    }
}
