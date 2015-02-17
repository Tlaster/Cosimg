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
        public async static Task<string> HttpReadStringWithPostMethod(string uriString)
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

        public async static Task<string> HttpReadStringWithPostString(string uriString, string postStr, string contentType)
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

        public async static Task<string> HttpReadStringWithCookie(string uriString, string cookie)
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

        public async static Task<string> HttpReadString(string uriString)
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
