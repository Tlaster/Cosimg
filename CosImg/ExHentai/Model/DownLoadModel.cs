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
using Windows.Web.Http;
using System.Diagnostics;

namespace CosImg.ExHentai.Model
{
    public class DownLoadModel :TBase.NotifyPropertyChanged
    {
        private HttpClient _client;
        private StorageFolder _saveFolder;
        private string _pageUri;
        private List<ImageListInfo> _imagePageUri;
        //private DetailProp _itemInfo;

        public int MaxImageCount { get; set; }
        public int CurrentPage { get; set; }
        public int CurrentPageProgress { get; set; }
        public string Name { get; set; }
        public BitmapImage ItemImage { get; set; }

        public DownLoadModel(string pageUri, StorageFolder saveFolder, string name)
        {
            _imagePageUri = new List<ImageListInfo>();
            _pageUri = pageUri;
            _saveFolder = saveFolder;
            Name = name;
            _client = new HttpClient();
            _client.DefaultRequestHeaders.Add("Cookie", SettingHelpers.GetSetting<string>("cookie"));
            CurrentPage = 0;
            StartDownLoad();
        }

        private async void StartDownLoad()
        {
            await GetImagePageListAsync();
            await DownloadFromUriList();
        }

        private async Task DownloadFromUriList()
        {
            var file = await _saveFolder.CreateFileAsync(this.CurrentPage.ToString(), CreationCollisionOption.ReplaceExisting);
            var sourceUri = await ParseHelper.GetImageAync(_imagePageUri[CurrentPage].ImagePage, SettingHelpers.GetSetting<string>("cookie"));
            var stream = await _client.GetInputStreamAsync(new Uri(sourceUri));
            using (var streamResponse = stream.AsStreamForRead())
            {
                using (Stream streamFile = await file.OpenStreamForWriteAsync())
                {
                    int bytesRead = 0;
                    byte[] myBuffer = new byte[1024 * 1024];

                    while ((bytesRead = await streamResponse.ReadAsync(myBuffer, 0, myBuffer.Length)) > 0)
                    {
                        Debug.WriteLine("CurrentPage:" + CurrentPage + " downloaded:" + bytesRead.ToString());
                        streamFile.Write(myBuffer, 0, bytesRead);
                    }
                }
            }



            
            //var res = await _client.GetAsync(new Uri(sourceUri));
            //if (!res.IsSuccessStatusCode)
            //{
            //    await DownloadFromUriList();
            //}
            //else
            //{
            //    var stream = await res.Content.ReadAsInputStreamAsync();
            //    using (var resStream = stream.AsStreamForRead())
            //    {
            //        using (var fileStream = await file.OpenStreamForWriteAsync())
            //        {
            //            int bytesRead = 0;
            //            ulong length;
            //            res.Content.TryComputeLength(out length);
            //            byte[] myBuffer = new byte[1024 * 1024];
            //            while ((bytesRead = resStream.Read(myBuffer, 0, myBuffer.Length)) > 0)
            //            {
            //                fileStream.Write(myBuffer, 0, bytesRead);
            //                Debug.WriteLine("length :" + length + " downloaded:" + bytesRead.ToString());
            //                //CurrentPageProgress = bytesRead / length;
            //                //OnPropertyChanged("CurrentPageProgress");
            //            }
            //        }
            //    }
            //}

            CurrentPage++;
            OnPropertyChanged("CurrentPage");
            if (CurrentPage < MaxImageCount)
            {
                await DownloadFromUriList();
            }
            else
            {
                CurrentPageProgress = 100;
                OnPropertyChanged("CurrentPageProgress");
                _client.Dispose();
                App.DownLoadList.Remove(this);
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
