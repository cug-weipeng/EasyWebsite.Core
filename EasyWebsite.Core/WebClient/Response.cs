using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyWebsite.Core.WebClient
{
    public class Response
    {
        public Response()
        {

        }

        public Response(ResponseTypeEnum responseType,object content)
        {
            ResponseType = responseType;
            Content = content;
        }

        public Response(ResponseTypeEnum responseType, object content, string cookie) : this(responseType, content)
        {
            Cookie = cookie;
        }

        public ResponseTypeEnum ResponseType { get; set; }

        public Object Content { get; set; }

        public string Cookie { get; set; }
    }

    public enum ResponseTypeEnum
    {
        Text = 0,
        Redirect = 1,
        Error = 2
    }
}
