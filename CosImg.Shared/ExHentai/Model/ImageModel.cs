using ExHentaiLib.Common;
using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;
using System.IO;
using CosImg.Common;
using TBase.RT;
using System.Windows.Input;
using TBase;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Net.Http;
using Windows.ApplicationModel.DataTransfer;
using Windows.Storage.Streams;
using System.Net.NetworkInformation;
using CosImg.ExHentai.Common;

namespace CosImg.ExHentai.Model
{
    public class ImageModel:TBase.NotifyPropertyChanged
    {



        #region StateModel
        private bool _isOnLoading;

        public bool isOnLoading
        {
            get { return _isOnLoading; }
            set { _isOnLoading = value; OnPropertyChanged("isOnLoading"); }
        }
        private bool _isLoadFail;

        public bool isLoadFail
        {
            get { return _isLoadFail; }
            set { _isLoadFail = value; OnPropertyChanged("isLoadFail"); }
        }
        double _width = (Window.Current.Content as Frame).ActualWidth;
#if WINDOWS_APP
        public double Height
        {
            get
            {
                return (Window.Current.Content as Frame).ActualHeight;
            }
        }
#endif
        public double Width
        {
            get
            {
                return _width;
            }
        }



        #endregion

#if WINDOWS_PHONE_APP
        public ImageModel()
        {
            RegisterForShare();
        }
#endif

        private async void GetDownloadedImage(int ImageIndex)
        {
            isOnLoading = true;
            try
            {
                byte[] _imagebyte = await ImageHelper.GetDownLoadedImage(SaveFolder, ImageIndex.ToString());
                _image = new WeakReference(await ImageHelper.ByteArrayToBitmapImage(_imagebyte));
                OnPropertyChanged("Image");
                isOnLoading = false;
            }
            catch (Exception)
            {
                isOnLoading = false;
                GetBitmapImage(ImagePage);
            }
        }
        async void GetBitmapImage(string uri)
        {
            if (isOnLoading || isLoadFail)
            {
                return;
            }
            isOnLoading = true;
            Debug.WriteLine("req page " + this.ImageIndex);
            try
            {
                if (ImagePage==null)
                {
                    throw new ArgumentNullException();
                }
                byte[] _imagebyte;
                if (await ImageHelper.CheckCacheImage(SaveFolder, ImageIndex.ToString()))
                {
                    _imagebyte = await ImageHelper.GetCacheImage(SaveFolder, ImageIndex.ToString());

                }
                else
                {
                    using (HttpClient _client = new HttpClient())
                    {
                        _client.Timeout = TimeSpan.FromSeconds(20d);
                        _client.DefaultRequestHeaders.Add("Cookie", SettingHelpers.GetSetting<string>("cookie")); 
                        if (_imageuri == null)
                        {
                            _imageuri = await ParseHelper.GetImageAync(uri, SettingHelpers.GetSetting<string>("cookie"));
                        }
                        using (var res = await _client.GetAsync(_imageuri))
                        {
                            if (res.Content.Headers.Contains("Content-Disposition"))
                            {
                                throw new HttpRequestException();
                            }
                            else
                            {
                                _imagebyte = await res.Content.ReadAsByteArrayAsync();
                            }
                        }
                        if (await DownLoadDBHelpers.CheckItemisDownloaded(SaveFolder))
                        {
                            await ImageHelper.SaveDownLoadedImage(SaveFolder, ImageIndex.ToString(), _imagebyte);
                        }
                        else
                        {
                            await ImageHelper.SaveCacheImage(SaveFolder, ImageIndex.ToString(), _imagebyte);
                        }
                    }
                }
                _image = new WeakReference(await ImageHelper.ByteArrayToBitmapImage(_imagebyte));
                OnPropertyChanged("Image");
            }
            catch (Exception)
            {
                isLoadFail = true;
            }
            isOnLoading = false;
        }
        public ICommand ReTryCommand
        {
            get
            {
                return new DelegateCommand(async () =>
                {
                    await Refresh();
                });
            }
        }
        public async Task Refresh()
        {
            if (ImagePage==null)
            {
                if (NetworkInterface.GetIsNetworkAvailable())
                {
                    new ToastPrompt("Loading info for this page,please wait").Show();
                }
                else
                {
                    new ToastPrompt("No info for this page").Show();
                }
            }
            else
            {
                isLoadFail = false;
                _image = null;
                _imageuri = null;
                await ImageHelper.DeleCacheImage(SaveFolder, ImageIndex.ToString());
                isDownLoaded = false;
                OnPropertyChanged("Image");
            }
        }

        public async void Save()
        {
            if (!await ImageHelper.CheckCacheImage(SaveFolder, ImageIndex.ToString()))
            {
                new ToastPrompt("Please wait while loading the image").Show();
                return;
            }
            await ImageHelper.SaveImage(Path.GetFileName(_imageuri) ?? SaveFolder + ImageIndex + ".jpg", await ImageHelper.GetCacheImage(SaveFolder, ImageIndex.ToString()));
        }

#if WINDOWS_PHONE_APP
        public async void Share()
        {
            if (!await ImageHelper.CheckCacheImage(SaveFolder, ImageIndex.ToString()))
            {
                new ToastPrompt("Please wait while loading the image").Show();
                return;
            } 
            DataTransferManager.ShowShareUI();
        }
        private void RegisterForShare()
        {
            DataTransferManager dataTransferManager = DataTransferManager.GetForCurrentView();
            dataTransferManager.DataRequested += ShareHandler;
        }

        private async void ShareHandler(DataTransferManager sender, DataRequestedEventArgs e)
        {
            DataRequestDeferral deferral = e.Request.GetDeferral();
            DataRequest request = e.Request;
            var _imagebyte = await ImageHelper.GetCacheImage(SaveFolder, this.ImageIndex.ToString());
            request.Data.Properties.Title = "Share Image";
            InMemoryRandomAccessStream randonAcc = new InMemoryRandomAccessStream();
            IOutputStream outputStream = randonAcc.GetOutputStreamAt(0);
            DataWriter writer = new DataWriter(outputStream);
            writer.WriteBytes(_imagebyte);
            await writer.StoreAsync();
            await writer.FlushAsync();
            RandomAccessStreamReference streamRef = RandomAccessStreamReference.CreateFromStream(randonAcc);
            request.Data.SetBitmap(streamRef);
            deferral.Complete();
        }


#endif


        public BitmapImage Image
        {
            get
            {
                if (_image != null && _image.IsAlive)
                {
                    return (BitmapImage)_image.Target;
                }
                else
                {
                    if (isDownLoaded)
                    {
                        GetDownloadedImage(ImageIndex);
                    }
                    else
                    {
                        GetBitmapImage(ImagePage);
                    }
                    return null;
                }
            }
        }
        //HttpClient _client;
        //WeakReference _client;
        WeakReference _image;
        private string _imageuri;
        public string ImagePage;
        public string SaveFolder;
        public int ImageIndex;
        public bool isDownLoaded;
    }
}
