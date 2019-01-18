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
        public HttpMethod Method { get; set; }
        private NetworkClient _HttpClient { get; set; } 

        public PageProcessorBase()
        {
            Method = HttpMethod.Get;
            NetworkClient netCenter = new NetworkClient() { AllowAutoRedirect = false };
        }

        public void Process()
        {
            ProcessOnce(Url);
        }

        private void ProcessOnce(string url)
        {
            Response response = WebRequest();
            Handle(response);
            if (NextPage.HasNextPage(response))
            {
                ProcessOnce(NextPage.nextUrl);
            }
        }

        abstract protected Response WebRequest();

        protected abstract void Handle(Response response);

    }

    public enum HttpMethod
    {
        Get = 0,
        Post = 1
    }
}
