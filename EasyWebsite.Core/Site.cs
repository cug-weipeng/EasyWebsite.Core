using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using EasyWebsite.Core.WebClient;
using EasyWebsite.Core.PageProcessor;

namespace EasyWebsite.Core
{
    public class Site : IDisposable
    {
        public Uri Domain { get;set; } 

        protected ExCookieCollection Cookies { get; set; }

        private string _LoginUrl = null;

        public Site(string domain)
        {
            Domain = new Uri(domain);
        }
        public Site( )
        {
        }

        public bool Login(string loginPath, object userObject)
        {
            string userJson = JsonConvert.SerializeObject(userObject);
            return Login(loginPath, userJson);
        }

        public bool Login(string loginPath,string userJson)
        {
            _LoginUrl = loginPath;
            HttpRequestClient netCenter = new HttpRequestClient() { AllowAutoRedirect = false };
            var response = netCenter.PostResponse(loginPath, userJson);
            if (response.Cookies.IsAspAuthenticate)
            {
                this.Cookies = response.Cookies;
                return true;
            }
            return false;
        }
        
        public void OpenPage(string url,IPageProcessor pageProcessor)
        {
            HttpRequestClient netCenter = new HttpRequestClient() { AllowAutoRedirect = false };
            var response = netCenter.GetResponse();

        }

        public void Dispose()
        {

        }
    }
}
