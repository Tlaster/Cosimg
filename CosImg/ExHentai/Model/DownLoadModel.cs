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
        private string _pageUri;


        public DownLoadModel(string pageUri, StorageFolder saveFolder, string name,byte[] imgbyte)
        {
            _imagebyte = imgbyte;
            _imagePageUri = new List<ImageListInfo>();
            _pageUri = pageUri;
            _saveFolder = saveFolder;
            Name = name;
            _client = new HttpClient();
            _client.DefaultRequestHeaders.Add("Cookie", SettingHelpers.GetSetting<string>("cookie"));
            CurrentPage = 0;
            HashString = name.GetHashedString();
            StartDownLoad();
        }

        public DownLoadModel(DownLoadInfo item)
        {
            base.CurrentPage = item.CurrentPage;
            base._saveFolder = item._saveFolder;
            base.HashString = item.HashString;
            base.Name = item.Name;
            base.MaxImageCount = item.MaxImageCount;
            base._imagePageUri = item._imagePageUri;
        }


        public void Parse()
        {

        }


        private async void StartDownLoad()
        {
            if (MaxImageCount == null || _imagePageUri == null)
            {
                await GetImagePageListAsync();
            }
            await DownloadFromUriList();
        }

        private async Task DownloadFromUriList()
        {
            var sourceUri = await ParseHelper.GetImageAync(_imagePageUri[CurrentPage].ImagePage, SettingHelpers.GetSetting<string>("cookie"));
            var file = await _saveFolder.CreateFileAsync(this.CurrentPage.ToString(), CreationCollisionOption.ReplaceExisting);

            var res = await _client.GetAsync(new Uri(sourceUri));
            if (!res.IsSuccessStatusCode)
            {
                await DownloadFromUriList();
            }
            else
            {
                using (var resStream = await res.Content.ReadAsStreamAsync())
                {
                    var resbyte = await Converter.StreamToBytes(resStream);
                    await FileIO.WriteBytesAsync(file, resbyte);
                    Debug.WriteLine("page" + CurrentPage + "completed");
                }
            }

            CurrentPage++;
            OnPropertyChanged("CurrentPage");
            DownLoadDBHelpers.Modify(HashString);
            if (CurrentPage < MaxImageCount)
            {
                await DownloadFromUriList();
            }
            else
            {
                _client.Dispose();
                App.DownLoadList.Remove(this);
                DownLoadDBHelpers.Delete(HashString);
            }
        }

        private async Task GetImagePageListAsync()
        {
            MaxImageCount = (await ParseHelper.GetDetailAsync(_pageUri, SettingHelpers.GetSetting<string>("cookie"))).MaxImageCount;
            OnPropertyChanged("MaxImageCount");
            _imagePageUri = await ParseHelper.GetImagePageListAsync(_pageUri, SettingHelpers.GetSetting<string>("cookie"));
        }


    }
}
