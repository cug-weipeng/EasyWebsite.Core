using EasyWebsite.Core.WebClient;
using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;

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
                return new Response(ResponseTypeEnum.Error, response.Content.ReadAsStreamAsync().GetAwaiter().GetResult(), response.Headers.GetCookieCollection());
            }

            if (response.IsRedirectCode())
            {
                
                return new Response(ResponseTypeEnum.Redirect, new Uri(response.RequestMessage.RequestUri, response.Headers.Location).ToString(), response.Headers.GetCookieCollection());
            }

            if(response.TryGetAttachmentName(out string fileName))
            {
                var result = new Response(ResponseTypeEnum.File, response.Content.ReadAsByteArrayAsync().GetAwaiter().GetResult(), response.Headers.GetCookieCollection());
                result.DownLoadFileName = fileName;
                return result;
            }

            return new Response(ResponseTypeEnum.Error, response.StatusCode.ToString(), response.Headers.GetCookieCollection());
        }
        
        private void SetSecurityProtocol(string url)
        {
            if (url.StartsWith("https"))
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls;
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
            httpContent.Headers.ContentType = new MediaTypeHeaderValue(MEDIATYPE_JSON);
            httpContent.Headers.ContentType.CharSet = "utf-8";
            return httpContent;
        }
    }
}
