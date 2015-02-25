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
using System.Diagnostics;

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

        public double Width
        {
            get
            {
                return (Window.Current.Content as Frame).ActualWidth;
            }
        }
#endregion

        async void GetImageBitmapImage(string uri)
        {
            try
            {
                isOnLoading = true;
                byte[] _imagebyte = default(byte[]);
                if (await ImageHelper.CheckCacheImage(SaveFolder, ImageIndex.ToString()))
                {
                    _imagebyte = await ImageHelper.GetCacheImage(SaveFolder, ImageIndex.ToString());
                }
                else
                {
                    _imageuri = await ParseHelper.GetImageAync(uri, SettingHelpers.GetSetting<string>("cookie"));
                    _imagebyte = await ImageHelper.GetImageByteArrayFromUriAsync(_imageuri, SettingHelpers.GetSetting<string>("cookie"));
                    await ImageHelper.SaveCacheImage(SaveFolder, ImageIndex.ToString(), _imagebyte);
                }
                _image = new WeakReference(await ImageHelper.ByteArrayToBitmapImage(_imagebyte));
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
            isLoadFail = false;
            _image = null;
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
        public async void Share()
        {
            if (!await ImageHelper.CheckCacheImage(SaveFolder, ImageIndex.ToString()))
            {
                new ToastPrompt("Please wait while loading the image").Show();
                return;
            }
            await ImageHelper.ShareImage(await ImageHelper.GetCacheImage(SaveFolder, ImageIndex.ToString()));
        }

        public BitmapImage Image
        {
            get
            {
                if (_image == null)
                {
                    GetImageBitmapImage(ImagePage);
                    Debug.WriteLine("Load");
                    return null;
                }
                else
                {
                    if (_image.IsAlive)
                    {
                        Debug.WriteLine("Get");
                        return (BitmapImage)_image.Target;
                    }
                    else
                    {
                        GetImageBitmapImage(ImagePage);
                        Debug.WriteLine("Reload");
                        return null;
                    }
                }
            }
        }

        WeakReference _image;
        private string _imageuri;
        public string ImagePage;
        public string SaveFolder;
        public int ImageIndex;
    }
}
