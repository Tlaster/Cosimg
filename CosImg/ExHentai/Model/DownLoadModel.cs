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
            var cachefolder = await ApplicationData.Current.LocalCacheFolder.CreateFolderAsync("download", CreationCollisionOption.OpenIfExists);
            _saveFolder = await cachefolder.CreateFolderAsync(HashString, CreationCollisionOption.OpenIfExists);
            await GetImagePageListAsync();
            await DownloadFromUriList();
        }


        int retryCount = 0;
        private async Task DownloadFromUriList()
        {
            try
            {
                var sourceUri = await ParseHelper.GetImageAync(_imagePageUri[CurrentPage].ImagePage, SettingHelpers.GetSetting<string>("cookie"));
                var file = await _saveFolder.CreateFileAsync(this.CurrentPage.ToString(), CreationCollisionOption.ReplaceExisting);

                //Debug.WriteLine("start page " + CurrentPage);
                var res = await _client.GetAsync(new Uri(sourceUri));
                if (!res.IsSuccessStatusCode)
                {
                    await Retry();
                }
                else
                {
                    using (var resStream = await res.Content.ReadAsStreamAsync())
                    {
                        var resbyte = await Converter.StreamToBytes(resStream);
                        await FileIO.WriteBytesAsync(file, resbyte);
                        //Debug.WriteLine("page " + CurrentPage + " completed");
                    }
                }
            }
            catch (Exception)
            {
                Retry();
            }

            await ToNext();
        }

        private async Task Retry()
        {
            retryCount++;
            if (retryCount == 5)
            {
                retryCount = 0;
                await ToNext();
            }
            else
            {
                _imagePageUri[CurrentPage].ImagePage += "&nl=" + random.Next(61, 63);
                //Debug.WriteLine("Fail,retry");
                await DownloadFromUriList();
            }
        }

        private async Task ToNext()
        {
            if (App.DownLoadList.Find((a) => { return a.HashString == base.HashString; }) == null)
            {
                //Debug.WriteLine("Cancel download");
                DownLoadDBHelpers.Delete(HashString);
                await ImageHelper.DeleDownloadFile(HashString);
            }
            else
            {
                CurrentPage++;
                OnPropertyChanged("CurrentPage");
                var item = await DownLoadDBHelpers.Query(HashString);
                item.CurrentPage = CurrentPage;
                DownLoadDBHelpers.Modify(item);
                if (CurrentPage < MaxImageCount)
                {
                    await DownloadFromUriList();
                }
                else
                {
                    //Debug.WriteLine("download complete");
                    _client.Dispose();
                    App.DownLoadList.Remove(this);
                    item.DownLoadComplete = true;
                    DownLoadDBHelpers.Modify(item);
                }

            }
        }

        private async Task GetImagePageListAsync()
        {
            MaxImageCount = (await ParseHelper.GetDetailAsync(PageUri, SettingHelpers.GetSetting<string>("cookie"))).MaxImageCount;
            OnPropertyChanged("MaxImageCount");
            _imagePageUri = await ParseHelper.GetImagePageListAsync(PageUri, SettingHelpers.GetSetting<string>("cookie"));
        }


    }
}
