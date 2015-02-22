using ExHentaiLib.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage.Streams;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Web.Http;
using System.IO;
using CosImg.Common;
using Windows.Storage;
using TBase.RT;
using System.Windows.Input;
using TBase;

namespace CosImg.ExHentai.Model
{
    public class ImageModel:TBase.NotifyPropertyChanged
    {
        private byte[] _imagebyte;

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

        public double Width
        {
            get
            {
                return (Window.Current.Content as Frame).ActualWidth;
            }
        }
        public string ImagePage { get; set; }
        async void GetImageBitmapImage(string uri)
        {
            try
            {
                isOnLoading = true;
                _imageuri = await ParseHelper.GetImageAync(uri, SettingHelpers.GetSetting<string>("cookie"));
                _imagebyte = await ImageHelper.GetImageByteArrayFromUriAsync(_imageuri, SettingHelpers.GetSetting<string>("cookie"));
                _image = await ImageHelper.ByteArrayToBitmapImage(_imagebyte);
                isOnLoading = false;
                OnPropertyChanged("Image");
            }
            catch (Exception)
            {
                isOnLoading = false;
                isLoadFail = true;
            }
        }
        public ICommand ReTryCommand
        {
            get
            {
                return new DelegateCommand(() =>
                {
                    Refresh();
                });
            }
        }
        public void Refresh()
        {
            _image = null;
            Random random = new Random();
            GetImageBitmapImage(ImagePage + "?nl=" + random.Next(61, 63).ToString());
        }
        public async void Save()
        {
            if (isOnLoading)
            {
                new ToastPrompt("Please wait while loading the image").Show();
                return;
            }
            await ImageHelper.SaveImage(Path.GetFileName(_imageuri),_imagebyte);
        }
        public async void Share()
        {
            if (isOnLoading)
            {
                new ToastPrompt("Please wait while loading the image").Show();
                return;
            }
            await ImageHelper.ShareImage(_imagebyte);
        }
        public BitmapImage Image
        {
            get
            {
                if (_image==null)
                {
                    GetImageBitmapImage(ImagePage);
                }
                return _image;
            }
        }
        BitmapImage _image;
        private string _imageuri;

        //public string _image;
    }
}
