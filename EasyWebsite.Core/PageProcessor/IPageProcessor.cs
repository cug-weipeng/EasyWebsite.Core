using EasyWebsite.Core.WebClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyWebsite.Core.PageProcessor
{
    public interface IPageProcessor
    {
        void Process(Response response);
    }
}
