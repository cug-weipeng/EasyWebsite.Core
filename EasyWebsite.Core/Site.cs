using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace EasyWebsite.Core
{
    public class Site : IDisposable
    {
        public Uri Domain { get;set; } 

        protected string Cookies { get; set; }

        private string _LoginUrl = null;

        public Site(string domain)
        {
            Domain = new Uri(domain);
        }

        public void Login(string loginPath, object userObject)
        {
            string userJson = JsonConvert.SerializeObject(userObject);
            Login(loginPath, userJson);
        }

        public bool Login(string loginPath,string userJson)
        {
            _LoginUrl = loginPath;
            NetworkClient netCenter = new NetworkClient();
            var response = netCenter.PostResponse(loginPath, userJson);
            if(response.ResponseType == WebClient.ResponseTypeEnum.Text)
            {
            }
        }
        
        public void OpenPage(string url)
        {

        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
