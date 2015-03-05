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
using Windows.Web.Http;

namespace CosImg.ExHentai.Model
{
    public class DownLoadModel :TBase.NotifyPropertyChanged
    {
        private HttpClient _client;
        private StorageFolder _saveFolder;
        private string _pageUri;
        private List<ImageListInfo> _imagePageUri;
        //private DetailProp _itemInfo;

        public int MaxImageCount { get; private set; }
        public int CurrentPage { get; private set; }
        public int CurrentPageProgress { get; private set; }
        public string Name { get; private set; }
        public BitmapImage ItemImage { get; private set; }

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


            //HttpResponseMessage responseMessage = await _client.GetAsync(sourceUri, HttpCompletionOption.ResponseHeadersRead);
            //long? contentLength = responseMessage.Content.Headers.ContentLength;
            //using (var fileStream = await file.OpenStreamForWriteAsync())
            //{
            //    int totalNumberOfBytesRead = 0;
            //    using (var responseStream =  await responseMessage.Content.ReadAsStreamAsync())
            //    {
            //        int numberOfReadBytes;
            //        do
            //        {
            //            // Read a data block into the buffer
            //            const int bufferSize = 1048576; // 1MB
            //            byte[] responseBuffer = new byte[bufferSize];
            //            numberOfReadBytes = await responseStream.ReadAsync(
            //               responseBuffer, 0, responseBuffer.Length);
            //            totalNumberOfBytesRead += numberOfReadBytes;
            //            // Write the data block into the file stream
            //            fileStream.Write(responseBuffer, 0, numberOfReadBytes);
            //            // Calculate the progress
            //            if (contentLength.HasValue)
            //            {
            //                // Calculate the progress
            //                double progressPercent = (totalNumberOfBytesRead /
            //                    (double)contentLength) * 100;
            //                // Display the progress
            //                Debug.WriteLine("CurrentPage:" + CurrentPage + " downloaded:" + progressPercent);
            //            }
            //            else
            //            {
            //                // Just display the read bytes   
            //            }
            //        } while (numberOfReadBytes != 0);
            //    }
            //}


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
