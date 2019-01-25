using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using DotnetSpider.Core;
using DotnetSpider.Core.Scheduler;
using DotnetSpider.Downloader;
using EasyWebsite.ConsoleTest.PageProcessor;
using Newtonsoft.Json;

namespace EasyWebsite.ConsoleTest
{
    class Program
    {
        static void Main(string[] args)
        {
            string url = @"http://css.cps.qqt-dev.com/account/logon";
            var procesor = new LoginProcessor();
            Spider spider = Spider.Create(
                          new QueueDuplicateRemovedScheduler(), procesor);
            
            ICookieInjector cookieInjector = null;
            spider.Downloader = new HttpClientDownloader() { CookieInjector = cookieInjector , AllowAutoRedirect = false };
            spider.Downloader.AddAfterDownloadCompleteHandler(new SetCookieHandler());
            string loginJson = JsonConvert.SerializeObject(new { Username = "weipeng", Password = "QQT_Wei123" });
            Request request = new Request(url);
            request.ContentType = "application/json";
            request.Content = loginJson;
            request.Method = HttpMethod.Post;

            //request.AddHeader("Cookie", cookie);
            spider.AddRequest(request);
            spider.Run();
        }
    }

    class SetCookieHandler : AfterDownloadCompleteHandler
    {
        public override void Handle(ref Response response, IDownloader downloader)
        {
        }
    }
}
