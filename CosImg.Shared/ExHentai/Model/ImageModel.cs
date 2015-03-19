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
#if WINDOWS_APP
        double _width = (Window.Current.Content as Frame).ActualWidth / 2;
        public double Height
        {
            get
            {
                return (Window.Current.Content as Frame).ActualHeight;
            }
        }
#else
        double _width = (Window.Current.Content as Frame).ActualWidth;
#endif
        public double Width
        {
            get
            {
                return _width;
            }
        }



        #endregion



        private async void GetImageBitmapImage(int ImageIndex)
        {
            isOnLoading = true;
            try
            {
                byte[] _imagebyte = await ImageHelper.GetDownLoadedImage(SaveFolder, ImageIndex.ToString());
                _image = new WeakReference(await ImageHelper.ByteArrayToBitmapImage(_imagebyte));
                OnPropertyChanged("Image");
            }
            catch (Exception)
            {
                GetBitmapImage(ImagePage);
            }

            isOnLoading = false;
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
                if (_client == null)
                {
                    _client = new HttpClient();
                    _client.DefaultRequestHeaders.Add("Cookie", SettingHelpers.GetSetting<string>("cookie"));
                }

                byte[] _imagebyte;
                if (await ImageHelper.CheckCacheImage(SaveFolder, ImageIndex.ToString()))
                {
                    _imagebyte = await ImageHelper.GetCacheImage(SaveFolder, ImageIndex.ToString());
                    
                }
                else
                {
                    if (_imageuri==null)
                    {
                        _imageuri = await ParseHelper.GetImageAync(uri, SettingHelpers.GetSetting<string>("cookie"));
                    }
                    _imagebyte = await _client.GetByteArrayAsync(_imageuri);
                    await ImageHelper.SaveCacheImage(SaveFolder, ImageIndex.ToString(), _imagebyte);
                }
                var bitimg = await ImageHelper.ByteArrayToBitmapImage(_imagebyte);
#if WINDOWS_APP
                double pwidth = bitimg.PixelWidth;
                double pheight = bitimg.PixelHeight;
                _width = pwidth / pheight * (Window.Current.Content as Frame).ActualHeight;
                OnPropertyChanged("Width");
#endif
                _image = new WeakReference(bitimg);
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
            isLoadFail = false;
            _image = null;
            _imageuri = null;
            await ImageHelper.DeleCacheImage(SaveFolder, ImageIndex.ToString());
            isDownLoaded = false;
            OnPropertyChanged("Image");
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
            await ImageHelper.ShareImage(await ImageHelper.GetCacheImage(SaveFolder, ImageIndex.ToString()));
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
                        GetImageBitmapImage(ImageIndex);
                    }
                    else
                    {
                        GetBitmapImage(ImagePage);
                    }
                    return null;
                }
            }
        }
        HttpClient _client;
        //WeakReference _client;
        WeakReference _image;
        private string _imageuri;
        public string ImagePage;
        public string SaveFolder;
        public int ImageIndex;
        public bool isDownLoaded;
    }
}
