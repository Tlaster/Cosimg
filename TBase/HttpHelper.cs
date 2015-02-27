using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace TBase
{
    public static class HttpHelper
    {
        public static async Task<byte[]> GetByteArray(string imgUri)
        {
            byte[] bit;
            using (HttpClient client = new HttpClient())
            {
                bit = await client.GetByteArrayAsync(imgUri);
            }
            return bit;
        }
        public static async Task<byte[]> GetByteArray(string imgUri, string cookie)
        {
            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("Cookie", cookie);
                //client.DefaultRequestHeaders.Accept.TryParseAdd("text/html, application/xhtml+xml, */*");
                //client.DefaultRequestHeaders.AcceptEncoding.TryParseAdd("gzip, deflate");
                //client.DefaultRequestHeaders.AcceptLanguage.TryParseAdd("en-US,en;q=0.8,zh-Hans-CN;q=0.6,zh-Hans;q=0.4,ja;q=0.2");
                //client.DefaultRequestHeaders.UserAgent.TryParseAdd("Mozilla/5.0 (MSIE 9.0; Windows NT 6.3; WOW64; Trident/7.0; rv:11.0) like Gecko");
                return await client.GetByteArrayAsync(imgUri);
            }
        }

        public async static Task<string> GetStringWithPostMethod(string uriString)
        {
            HttpWebRequest req = HttpWebRequest.Create(uriString) as HttpWebRequest;
            req.Method = "POST";
            HttpWebResponse res = await req.GetResponseAsync() as HttpWebResponse;
            string returnStr = "";
            using (var getcontent = new StreamReader(res.GetResponseStream(), Encoding.UTF8))
            {
                returnStr = getcontent.ReadToEnd();
            }
            return returnStr;
        }

        public async static Task<string> GetStringWithPostString(string uriString, string postStr, string contentType)
        {
            string returnStr = "";
            byte[] data = Encoding.UTF8.GetBytes(postStr);
            HttpWebRequest req = WebRequest.Create(uriString) as HttpWebRequest;
            req.Method = "POST";
            req.ContentType = contentType;
            using (Stream stream = await req.GetRequestStreamAsync())
            {
                stream.Write(data, 0, data.Length);
            }
            using (var res = await req.GetResponseAsync() as HttpWebResponse)
            {
                using (var getcontent = new StreamReader(res.GetResponseStream(), Encoding.UTF8))
                {
                    returnStr = getcontent.ReadToEnd();
                }
            }
            return returnStr;
        }

        public async static Task<string> GetStringWithCookie(string uriString, string cookie)
        {
            string returnStr = "";
            HttpWebRequest webRequest = HttpWebRequest.Create(uriString) as HttpWebRequest;
            webRequest.Headers["Cookie"] = cookie;
            using (HttpWebResponse webResponse = await webRequest.GetResponseAsync() as HttpWebResponse)
            {
                using (var getContent = new StreamReader(webResponse.GetResponseStream(), Encoding.UTF8))
                {
                    returnStr = await getContent.ReadToEndAsync();
                }
            }
            return returnStr;
        }

        public async static Task<string> GetReadString(string uriString)
        {
            string returnStr = "";
            using (HttpClient client = new HttpClient())
            {
                returnStr = await (await client.GetAsync(new Uri(uriString))).Content.ReadAsStringAsync();
            }
            return returnStr;
        }
    }
}
