using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media;
using ExHentaiLib.Common;
using ExHentaiLib.Interface;
using ExHentaiLib.Prop;
using ExHentaiViewer.WPF.Prop;
using TBase;

namespace ExHentaiViewer.WPF.ViewModel
{
    public class ReadingViewModel:BaseViewModel
    {
        private List<ImageListInfo> imageList;

        private string _currentImage;
        private int MaxImageCount;
        private int CurrentImageIndex = 0;

        public string CurrentImage
        {
            get { return _currentImage; }
            set
            {
                _currentImage = value;
                OnPropertyChanged("CurrentImage");
            }
        }

        public ReadingViewModel(string uri)
        {
            OnLoaded(uri);
        }
        

        public ReadingViewModel(string uri, ImageListInfo info)
        {
            OnLoaded(uri, info);
        }

        private async void OnLoaded(string uri)
        {
            imageList = await ParseHelper.GetImagePageListAsync(uri, GetCookie());
            MaxImageCount = ParseHelper.GetMaxImageCount((await ParseHelper.GetDetailAsync(uri, GetCookie())).ImageCountString);
            CurrentImage = (await ImageListInfo2T(imageList[CurrentImageIndex])).ImageUri;
        }
        private async void OnLoaded(string uri, ImageListInfo info)
        {
            imageList = await ParseHelper.GetImagePageListAsync(uri, GetCookie());
            MaxImageCount = ParseHelper.GetMaxImageCount((await ParseHelper.GetDetailAsync(uri, GetCookie())).ImageCountString);
            CurrentImageIndex = imageList.FindIndex(a => a.ImageName == info.ImageName);
            CurrentImage = (await ImageListInfo2T(imageList[CurrentImageIndex])).ImageUri;
        }

        public async Task<ReadingImageProp> ImageListInfo2T(ImageListInfo doanloadprop)
        {
            return new ReadingImageProp() { ImageName = doanloadprop.ImageName, ImageUri = await ParseHelper.GetImageAync(doanloadprop.ImagePage, GetCookie()) };
        }
        
        public ICommand ClickNext
        {
            get
            {
                return new DelegateCommand(async() =>
                {
                    if (CurrentImageIndex<MaxImageCount)
                    {
                        CurrentImageIndex++;
                        CurrentImage = (await ImageListInfo2T(imageList[CurrentImageIndex])).ImageUri;
                    }
                });
            }
        }

        public ICommand ClickForward
        {
            get
            {
                return new DelegateCommand(async() =>
                {
                    if (CurrentImageIndex>0)
                    {
                        CurrentImageIndex--;
                        CurrentImage = (await ImageListInfo2T(imageList[CurrentImageIndex])).ImageUri;
                    }
                });
            }
        }



    }
}
