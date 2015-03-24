using ExHentaiLib.Common;
using ExHentaiLib.Prop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TBase.RT;
using Windows.Storage;
using System.IO;
using Windows.UI.Xaml.Media.Imaging;
using System.Diagnostics;
using Windows.Networking.BackgroundTransfer;
using System.Net.Http;
using TBase;
using CosImg.ExHentai.Common;
using CosImg.Common;

namespace CosImg.ExHentai.Model
{
    public class DownLoadModel : DownLoadInfo
    {
        private HttpClient _client;
        Random random;
        public StorageFolder _saveFolder { get; private set; }
        public List<ImageListInfo> _imagePageUri { get; private set; }


        public DownLoadModel(string pageUri, string name,byte[] imgbyte)
        {
            Imagebyte = imgbyte;
            _imagePageUri = new List<ImageListInfo>();
            PageUri = pageUri;
            Name = name;
            HashString = name.GetHashedString();
            _client = new HttpClient();
            _client.DefaultRequestHeaders.Add("Cookie", SettingHelpers.GetSetting<string>("cookie"));
            CurrentPage = 0;
            StartDownLoad();
        }

        public DownLoadModel(DownLoadInfo item)
        {
            base.Imagebyte = item.Imagebyte;
            base.CurrentPage = item.CurrentPage;
            base.Name = item.Name;
            base.MaxImageCount = item.MaxImageCount;
            base.HashString = item.Name.GetHashedString();
            base.PageUri = item.PageUri;
            _imagePageUri = new List<ImageListInfo>();
            _client = new HttpClient();
            _client.DefaultRequestHeaders.Add("Cookie", SettingHelpers.GetSetting<string>("cookie"));
            StartDownLoad();
        }




        private async void StartDownLoad()
        {
            random = new Random();
#if WINDOWS_PHONE_APP
            var cachefolder = await ApplicationData.Current.LocalCacheFolder.CreateFolderAsync("download", CreationCollisionOption.OpenIfExists);
#else
            var cachefolder = await ApplicationData.Current.LocalFolder.CreateFolderAsync("download", CreationCollisionOption.OpenIfExists);
#endif
            _saveFolder = await cachefolder.CreateFolderAsync(HashString, CreationCollisionOption.OpenIfExists);
            await GetImagePageListAsync();
            await DownloadFromUriList();
        }


        int retryCount = 0;
        private async Task DownloadFromUriList()
        {

            var item = await DownLoadDBHelpers.Query(HashString);
            for (; CurrentPage < MaxImageCount; CurrentPage++, OnPropertyChanged("CurrentPage"), item.CurrentPage = CurrentPage, DownLoadDBHelpers.Modify(item))
            {
                Debug.WriteLine("page " + CurrentPage + " start");
                var file = await _saveFolder.CreateFileAsync(this.CurrentPage.ToString(), CreationCollisionOption.ReplaceExisting);
                var sourceUri = await ParseHelper.GetImageAync(_imagePageUri[CurrentPage].ImagePage, SettingHelpers.GetSetting<string>("cookie"));
                var res = await _client.GetAsync(new Uri(sourceUri));
                if (!res.IsSuccessStatusCode || res.Content.Headers.Contains("Content-Disposition"))
                {
                    for (int i = 0; i < 5; i++)
                    {
                        Debug.WriteLine("page " + CurrentPage + " restart " + i);
                        _imagePageUri[CurrentPage].ImagePage += "&nl=" + random.Next(61, 63);
                        sourceUri = await ParseHelper.GetImageAync(_imagePageUri[CurrentPage].ImagePage, SettingHelpers.GetSetting<string>("cookie"));
                        res = await _client.GetAsync(new Uri(sourceUri));
                        if (res.IsSuccessStatusCode)
                        {
                            break;
                        }
                    }
                }
                if (res.IsSuccessStatusCode)
                {
                    using (var resStream = await res.Content.ReadAsStreamAsync())
                    {
                        var resbyte = await Converter.StreamToBytes(resStream);
                        await FileIO.WriteBytesAsync(file, resbyte);
                        Debug.WriteLine("page " + CurrentPage + " completed");
                    }
                }
                if (App.DownLoadList.Find((a) => { return a.HashString == base.HashString; }) == null)
                {
                    Debug.WriteLine("DownLoad Cancel");
                    DownLoadDBHelpers.Delete(HashString);
                    await ImageHelper.DeleDownloadFile(HashString);
                    break;
                }
            }
            if (App.DownLoadList.Find((a) => { return a.HashString == base.HashString; }) != null)
            {
                _client.Dispose();
                App.DownLoadList.Remove(this);
                item.DownLoadComplete = true;
                DownLoadDBHelpers.Modify(item);
            }

        }



        private async Task GetImagePageListAsync()
        {
            _imagePageUri = await ParseHelper.GetImagePageListAsync(PageUri, SettingHelpers.GetSetting<string>("cookie"));
            MaxImageCount = _imagePageUri.Count; 
            OnPropertyChanged("MaxImageCount");
        }


    }
}
