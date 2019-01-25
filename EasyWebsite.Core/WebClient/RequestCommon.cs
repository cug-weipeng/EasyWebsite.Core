using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyWebsite.Core.WebClient
{
    /// <summary>
    /// 一次请求中的通用项
    /// </summary>
    public class RequestCommon
    {
        public const string CHROME_AGENT = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/71.0.3578.98 Safari/537.36";
        public const string ACCEPT_DEFAULT = "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8";
        public const string CONTENTTYPE_FORM = "application/x-www-form-urlencoded";
        public const string CONTENTTYPE_JSON = "application/json";


        public string UserAgent { get; set; } = CHROME_AGENT;

        /// <summary>
        /// Accept
        /// </summary>
        public string Accept { get; set; } = ACCEPT_DEFAULT;

        /// <summary>
        /// 仅在发送 POST 请求时需要设置，默认为JSON
        /// </summary>
        public string ContentType { get; set; } = CONTENTTYPE_JSON;

        /// <summary>
        /// Headers
        /// </summary>
        public Dictionary<string, object> Headers { get; set; } = new Dictionary<string, object>();

        /// <summary>
        /// 字符编码
        /// </summary>
        public string EncodingName { get; set; } = "utf8";

        public bool AllowAutoRedirect { get; set; } = true;
    }
}
