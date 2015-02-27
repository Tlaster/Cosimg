using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ExHentaiLib.Common
{
    public static class LogInHelper
    {
        public static void LogCookieCheck(string cookie)
        {
            string memberidRegex = @"ipb_member_id=([^;]*)";
            string passhashRegex = @"ipb_pass_hash=([^;]*)";
            string igneousRegex = @"igneous=([^;]*)";
            var memberidStr = Regex.Match(cookie, memberidRegex);
            var passhashStr = Regex.Match(cookie, passhashRegex);
            var igneousStr = Regex.Match(cookie, igneousRegex);
            if (!memberidStr.Success || !passhashStr.Success || !igneousStr.Success)
            {
                throw new CookieException("Cookie Error");
            }
        }

        public static async Task<string> GetLoginCookie(string userName, string passWord)
        {
            string postStr = "UserName=" + userName + "&PassWord=" + passWord + "&x=0&y=0";
            byte[] data = Encoding.UTF8.GetBytes(postStr);
            HttpWebRequest loginRequest = HttpWebRequest.CreateHttp("http://forums.e-hentai.org/index.php?act=Login&CODE=01&CookieDate=1 ");
            loginRequest.Method = "POST";
            loginRequest.ContentType = "application/x-www-form-urlencoded";
            using (Stream stream = await loginRequest.GetRequestStreamAsync())
            {
                stream.Write(data, 0, data.Length);
            }
            string logCookie = "";
            using (HttpWebResponse logResponse = (HttpWebResponse)(await loginRequest.GetResponseAsync()))
            {
                logCookie = logResponse.Headers["Set-Cookie"];
            }
            string memberidRegex = @"ipb_member_id=([^;]*)";
            string passhashRegex = @"ipb_pass_hash=([^;]*)";
            var memberidStr = Regex.Match(logCookie, memberidRegex);
            var passhashStr = Regex.Match(logCookie, passhashRegex);
            if (!memberidStr.Success||!passhashStr.Success)
            {
                throw new LoginException("Login Error");
            }
            HttpWebRequest webRequest = HttpWebRequest.CreateHttp("http://exhentai.org/");
            webRequest.Headers["Cookie"] = memberidStr.Value + ";" + passhashStr.Value;
            string imgCookie = "";
            using (HttpWebResponse webResponse = await webRequest.GetResponseAsync() as HttpWebResponse)
            {
                if (webResponse.ContentType=="image/gif")
                {
                    throw new LogAccessException("No Access");
                }
                imgCookie = webResponse.Headers["Set-Cookie"];
            }
            string igneousRegex = @"igneous=([^;]*)";
            var igneousStr = Regex.Match(imgCookie, igneousRegex);
            return memberidStr.Value + ";" + passhashStr.Value + ";" + igneousStr.Value;
        }
    }
}
