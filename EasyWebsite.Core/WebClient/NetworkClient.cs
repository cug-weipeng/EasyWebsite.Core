using EasyWebsite.Core.WebClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace EasyWebsite.Core
{
    internal class NetworkClient
    {
        private const string MEDIATYPE_JSON = "application/json";
        private const string MEDIATYPE_FORM_URLENCODED = "application/x-www-form-urlencoded";
        
        public bool AllowAutoRedirect { get; set; }

        public Response GetResponse(string url)
        {
            SetSecurityProtocol(url);

            HttpClient httpClient = CreateHttpClient();

            return ConvertResponse(httpClient.GetAsync(url).Result);

        }

        public Response PostResponse(string url, string postData)
        {
            SetSecurityProtocol(url);

            var httpContent = CreatePostContent(postData);

            HttpClient httpClient = CreateHttpClient();

            HttpResponseMessage response = httpClient.PostAsync(url, httpContent).Result;

            return ConvertResponse(response);
        }

        private Response ConvertResponse(HttpResponseMessage response)
        {
            if (response.IsSuccessStatusCode)
            {
                string result = response.Content.ReadAsStringAsync().Result;
                return new Response(ResponseTypeEnum.Text, response);
            }
            else if (response.IsRedirectCode())
            {
                return new Response(ResponseTypeEnum.Redirect, new Uri(response.RequestMessage.RequestUri, response.Headers.Location).ToString());
            }
            else
            {
                return new Response(ResponseTypeEnum.Error, response.StatusCode.ToString());
            }
        }

        private void SetSecurityProtocol(string url)
        {
            if (url.StartsWith("https"))
                System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls;
        }

        private HttpClient CreateHttpClient()
        {
            var httpClient = new HttpClient(new HttpClientHandler() { AllowAutoRedirect = AllowAutoRedirect });
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(MEDIATYPE_JSON));
            return httpClient;
        }

        private HttpContent CreatePostContent(string postData)
        {
            HttpContent httpContent = new StringContent(postData);
            httpContent.Headers.ContentType = new MediaTypeHeaderValue(MEDIATYPE_FORM_URLENCODED);
            httpContent.Headers.ContentType.CharSet = "utf-8";
            return httpContent;
        }
    }
}
