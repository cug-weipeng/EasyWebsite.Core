using EasyWebsite.Core.WebClient;
using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;

namespace EasyWebsite.Core
{
    internal class HttpRequestClient
    {
        class HttpClientObject : IDisposable
        {
            public DateTime LastUseTime { get; set; }
            public HttpClient Client { get; }
            public HttpClientHandler Handler { get; }

            public HttpClientObject(HttpClientHandler handler, bool allowAutoRedirect)
            {
                Handler = handler;
                Client = new HttpClient(handler, true);
            }

            public void Dispose()
            {
                Client.Dispose();
            }
        }

        private readonly bool _decodeHtml;
        private readonly int _timeout;
        private static int _getHttpClientCount;
        private HttpClientObject _clientObject;

        public HttpRequestClient(int timeout = 8000, bool decodeHtml = false)
        {
            _timeout = timeout;
            _decodeHtml = decodeHtml;
        }

        protected Response DownloadContent(Request request)
        {
            var response = new Response(request);

            if (IfFileExists(request))
            {
                Logger?.LogInformation($"File {request.Url} already exists.");
                return response;
            }

            var httpRequestMessage = GenerateHttpRequestMessage(request);
            HttpResponseMessage httpResponseMessage = null;
            WebProxy proxy = null;
            try
            {
                if (UseFiddlerProxy)
                {
                    if (FiddlerProxy == null)
                    {
                        throw new DownloaderException("Fiddler proxy is null.");
                    }
                    else
                    {
                        proxy = FiddlerProxy;
                    }
                }
                else
                {
                    if (HttpProxyPool.Instance != null)
                    {
                        proxy = HttpProxyPool.Instance.GetProxy();
                        if (proxy == null)
                        {
                            throw new DownloaderException("No available proxy.");
                        }
                    }
                    else
                    {
                        _clientObject = GetHttpClient("DEFAULT", AllowAutoRedirect, null);
                    }
                }

                _clientObject = GetHttpClient(proxy == null ? "DEFAULT" : $"{proxy.Address}",
                    AllowAutoRedirect, proxy);

                httpResponseMessage =
                    NetworkCenter.Current.Execute("downloader", () => Task
                        .Run(async () => await _clientObject.Client.SendAsync(httpRequestMessage))
                        .GetAwaiter()
                        .GetResult());

                response.StatusCode = httpResponseMessage.StatusCode;
                EnsureSuccessStatusCode(response.StatusCode);
                response.TargetUrl = httpResponseMessage.RequestMessage.RequestUri.AbsoluteUri;

                var bytes = httpResponseMessage.Content.ReadAsByteArrayAsync().Result;
                if (!ExcludeMediaTypes.Any(t => httpResponseMessage.Content.Headers.ContentType.MediaType.Contains(t)))
                {
                    if (!DownloadFiles)
                    {
                        Logger?.LogWarning($"Ignore {request.Url} because media type is not allowed to download.");
                    }
                    else
                    {
                        StorageFile(request, bytes);
                    }
                }
                else
                {
                    var content = ReadContent(request, bytes,
                        httpResponseMessage.Content.Headers.ContentType.CharSet);

                    if (_decodeHtml && content is string)
                    {
#if NETFRAMEWORK
						content =
 System.Web.HttpUtility.UrlDecode(System.Web.HttpUtility.HtmlDecode(content.ToString()), string.IsNullOrEmpty(request.EncodingName) ? Encoding.UTF8 : Encoding.GetEncoding(request.EncodingName));
#else
                        content = WebUtility.UrlDecode(WebUtility.HtmlDecode(content.ToString()));
#endif
                    }

                    response.Content = content;

                    DetectContentType(response, httpResponseMessage.Content.Headers.ContentType.MediaType);
                }
            }
            catch (DownloaderException)
            {
                throw;
            }
            catch (Exception e)
            {
                throw new DownloaderException($"Unexpected exception when download request: {request.Url}: {e}.");
            }
            finally
            {
                if (HttpProxyPool.Instance != null && proxy != null)
                {
                    HttpProxyPool.Instance.ReturnProxy(proxy,
                        httpResponseMessage?.StatusCode ?? HttpStatusCode.ServiceUnavailable);
                }

                try
                {
                    httpResponseMessage?.Dispose();
                }
                catch (Exception e)
                {
                    throw new BypassedDownloaderException($"Close response {request.Url} failed: {e.Message}");
                }
            }

            return response;
        }

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
