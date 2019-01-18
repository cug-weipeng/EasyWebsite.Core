using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EasyWebsite.Core;

namespace EasyWebsite.ConsoleTest
{
    class Program
    {
        static void Main(string[] args)
        {
            Site site = new Site();
            if(site.Login("http://css.cps.qqt-dev.com/account/logon",new { Username="weipeng",Password="QQT_Wei123"}))
            {
                Console.WriteLine("ok");
            }
            else
            {
                Console.WriteLine("登录失败");
            }
            Console.ReadLine();
        }
    }
}
