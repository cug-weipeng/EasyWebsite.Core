using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyWebsite.Core.WebClient
{
    public class Request
    {
        public Request(RequestCommon common)
        {
            RequestCommon = common;
        }
        public RequestCommon RequestCommon { get; set; }

        public Uri Url { get; set; }

        public Uri Referer { get; set; }

        public string Origin { get; set; }
    }
}
