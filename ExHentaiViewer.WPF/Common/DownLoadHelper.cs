using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using ExHentaiLib.Common;

namespace ExHentaiViewer.WPF.Common
{
    public class WebDownload : WebClient
    {
        /// <summary>
        /// Time in milliseconds
        /// </summary>
        public int Timeout { get; set; }

        public WebDownload() : this(60000) { }

        public WebDownload(int timeout)
        {
            this.Timeout = timeout;
        }

        protected override WebRequest GetWebRequest(Uri address)
        {
            var request = base.GetWebRequest(address);
            if (request != null)
            {
                request.Timeout = this.Timeout;
            }
            return request;
        }
    }
    public sealed class DownLoadHelper
    {
        public WebDownload webClient;
        public event EventHandler<AsyncCompletedEventArgs> DownLoadCompleted;
        public event EventHandler DownLoadFailed;
        private int ReTryCount = 0;
        private const int CanReTryCunt = 5;
        public DownLoadHelper(string uri,string savePathName,string cookie)
        {
            //DownLoadAsync(uri, savePathName, cookie);
            TryDownLoadAsync(uri, savePathName,cookie);
        }

        private async void DownLoadAsync(string uri, string savePathName,string cookie)
        {
            var sourceUri = await ParseHelper.GetImageAync(uri, cookie);
            webClient = new WebDownload();
            webClient.Headers["Cookie"] = cookie;
            webClient.Headers["Referer"] = uri;
            webClient.Headers["Accept"] = "text/html, application/xhtml+xml, */*";
            webClient.Headers["Accept-Encoding"] = "gzip, deflate";
            webClient.Headers["Accept-Language"] = "en-US,en;q=0.8,zh-Hans-CN;q=0.6,zh-Hans;q=0.4,ja;q=0.2";
            webClient.Headers["User-Agent"] = "Mozilla/5.0 (MSIE 9.0; Windows NT 6.3; WOW64; Trident/7.0; rv:11.0) like Gecko";
            webClient.DownloadFileCompleted += webClient_DownloadFileCompleted;
            await Task.Delay(1000);
            await webClient.DownloadFileTaskAsync(new Uri(sourceUri), savePathName + System.IO.Path.GetExtension(sourceUri));
        }


        private async void TryDownLoadAsync(string uri, string savePathName, string cookie)
        {
            Random random = new Random();
            try
            {
                await DownLoadTaskAsync(uri, savePathName,cookie);
            }
            catch (Exception e)
            {
                if (ReTryCount < CanReTryCunt)
                {
                    ReTryCount++;
                    TryDownLoadAsync(uri + "?nl=" + random.Next(61, 63).ToString(), savePathName, cookie);
                }
                else
                {
                    if (DownLoadFailed!=null)
                    {
                        DownLoadFailed(e, null);
                    }
                }
            }
        }

        private async Task DownLoadTaskAsync(string uri, string savePathName, string cookie)
        {
            var sourceUri = await ParseHelper.GetImageAync(uri, cookie);
            webClient = new WebDownload();
            webClient.Timeout = 10000;
            webClient.Headers["Cookie"] = cookie;
            webClient.Headers["Referer"] = uri;
            webClient.Headers["Accept"] = "text/html, application/xhtml+xml, */*";
            webClient.Headers["Accept-Encoding"] = "gzip, deflate";
            webClient.Headers["Accept-Language"] = "en-US,en;q=0.8,zh-Hans-CN;q=0.6,zh-Hans;q=0.4,ja;q=0.2";
            webClient.Headers["User-Agent"] = "Mozilla/5.0 (MSIE 9.0; Windows NT 6.3; WOW64; Trident/7.0; rv:11.0) like Gecko";
            webClient.DownloadFileCompleted += webClient_DownloadFileCompleted; 
            await Task.Delay(1000);
            await webClient.DownloadFileTaskAsync(new Uri(sourceUri), savePathName + System.IO.Path.GetExtension(sourceUri));
            //To check the download file,but I can not sure it's the right solution
            for (int i = 0; i < webClient.ResponseHeaders.AllKeys.Length; i++)
            {
                if (webClient.ResponseHeaders.Get(i) == "Content-Disposition")
                {
                    throw new Exception();
                }
            }

        }

        private void webClient_DownloadFileCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
            webClient.Dispose();
            if (DownLoadCompleted!=null)
            {
                DownLoadCompleted(sender, e);
            }
        }
    }
}
