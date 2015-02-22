using ExHentaiLib.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExHentaiViewer.WPF.Common
{
    public static class CookieHelper
    {
        public static string GetCookie()
        {
            return File.ReadAllText(AppDomain.CurrentDomain.BaseDirectory + "cookie.cookie") + ParseHelper.unconfig;
        }

        public static void SsveCookie(string cookie)
        {
            File.WriteAllText(AppDomain.CurrentDomain.BaseDirectory + "cookie.cookie", cookie);
        }

    }
}
