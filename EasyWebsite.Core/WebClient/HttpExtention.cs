using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace EasyWebsite.Core.WebClient
{
    public static class HttpExtention
    {

        public static bool IsRedirectCode(this HttpResponseMessage response)
        {
            if (response.StatusCode == HttpStatusCode.Redirect
                   || response.StatusCode == HttpStatusCode.Found
                   || response.StatusCode == HttpStatusCode.SeeOther)
            {
                return true;
            }
            return false;
        }

        public static bool TryGetAttachmentName(this HttpResponseMessage response,out string fileName)
        {
            if(response.Headers.TryGetValues("content-Disposition" ,out IEnumerable<string> content_Dispositions))
            {
                string content_Disposition = content_Dispositions.FirstOrDefault();
                if (content_Disposition.Contains("filename="))
                {
                    fileName = content_Disposition.Split(new string[] { "filename=" }, StringSplitOptions.RemoveEmptyEntries)[1];
                    return true;
                }
                else if (content_Disposition.ToLower().Contains("filename*="))
                {
                    if (content_Disposition.ToLower().Contains("utf8") || content_Disposition.ToLower().Contains("utf-8"))
                    {
                        string contentDecoded = System.Web.HttpUtility.UrlDecode(content_Disposition, Encoding.UTF8);
                        int index = contentDecoded.LastIndexOf("'");
                        fileName = contentDecoded.Substring(index + 1);
                        return true;
                    }
                }
                throw new Exception($"无法识别文件名{content_Disposition}");
            }
            fileName = null;
            return false;
        }

    }
}
