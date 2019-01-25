using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EasyWebsite.Core.WebClient;

namespace EasyWebsite.Core.PageProcessor
{
    public abstract class PageProcessorBase : IPageProcessor
    {
        public string Url { get; set; } 
        private HttpRequestClient _HttpClient { get; set; } 

        public IWebRequester Requester { get; set; }

        public RequestCommon RequestPartial { get; set; }

        public PageProcessorBase(IWebRequester requester)
        {
            Requester = requester;
        }

        public void Process()
        {
            ProcessOnce(Url);
        }

        private void ProcessOnce(string url)
        {
            Request request = new Request(RequestPartial);
            request.Url = new Uri(url);

            Response response = Requester.Send(request);
            Handle(response);

            string nextPageUrl = GetNextPageUrl(response);
            if (!string.IsNullOrWhiteSpace(nextPageUrl))
            {
                ProcessOnce(nextPageUrl);
            }
        }
        
        protected abstract void Handle(Response response);

        /// <summary>
        /// 获取下一页的链接
        /// </summary>
        /// <param name="response"></param>
        /// <returns></returns>
        protected virtual string GetNextPageUrl(Response response)
        {
            return null;
        }
    }
}
