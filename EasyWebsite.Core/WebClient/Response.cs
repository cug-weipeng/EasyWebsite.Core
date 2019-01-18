using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
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

        public Response(ResponseTypeEnum responseType, object content, ExCookieCollection cookies) : this(responseType, content)
        {
            Cookies = cookies;
        }

        public ResponseTypeEnum ResponseType { get; set; }

        public Object Content { get; set; }

        public ExCookieCollection Cookies { get; set; }

        /// <summary>
        /// 如果想要结果为文件下载，这里保存下载文件的名称
        /// </summary>
        public string DownLoadFileName { get; set; }
    }

    public enum ResponseTypeEnum
    {
        Text = 0,
        Redirect = 1,
        Error = 2,
        File = 3
    }
}
