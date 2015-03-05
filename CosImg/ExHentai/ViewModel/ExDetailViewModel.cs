using CosImg.CosImg.Common;
using CosImg.ExHentai.Model;
using CosImg.ExHentai.View;
using ExHentaiLib.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using TBase;
using TBase.RT;
using Windows.UI.Popups;
using CosImg.Common;
using Windows.Storage;
using CosImg.ExHentai.Common;
using Windows.UI.Xaml.Controls;

namespace CosImg.ExHentai.ViewModel
{
    public class ExDetailViewModel : LoadProps
    {
        private string _link;
        private bool _favorState;

        public ExDetailViewModel(string link)
        {
            this._link = link;
            OnLoaded(link);
        }

        private async void OnLoaded(string link)
        {
            try
            {
                isOnLoading = true;
                Detail = await ParseHelper.GetDetailAsync(link, SettingHelpers.GetSetting<string>("cookie"));
                if (await FavorDBHelpers.CheckFavorDBFile())
                {
                    var item = await FavorDBHelpers.Query(Detail.HeaderInfo.TitleEn.GetHashedString());
                    _favorState = item == null ? false : true;
                }
                PageList = new List<PageListModel>();
                for (int i = 0; i < Detail.DetailPageCount; i++)
                {
                    PageList.Add(new PageListModel() { Page = (i + 1).ToString(), Uri = this._link + "?p=" + i });
                }
                isOnLoading = false;
            }
            catch (Exception)
            {
                isLoadFail = true;
                isOnLoading = false;
            }
        }

        private async void LoadMoreImage(string uri)
        {
            try
            {
                isImageOnLoading = true;
                var a = await ParseHelper.GetDetailAsync(uri, SettingHelpers.GetSetting<string>("cookie"));
                Detail.ImageList = a.ImageList;
                isImageOnLoading = false;
            }
            catch (Exception)
            {
                isImageOnLoading = false;
            }
        }

        public ICommand ViewInIECommand
        {
            get
            {
                return new DelegateCommand(async() =>
                {
                    await Windows.System.Launcher.LaunchUriAsync(new Uri(this._link));
                });
            }
        }

        public ICommand FavorCommand
        {
            get
            {
                return new DelegateCommand( async() =>
                {
                    if (_favorState)
                    {
                        FavorDBHelpers.Delete(Detail.HeaderInfo.TitleEn.GetHashedString());
                        _favorState = false;
                    }
                    else
                    {
                        FavorDBHelpers.Add(new FavorModel() 
                        { 
                            HashString = Detail.HeaderInfo.TitleEn.GetHashedString(),
                            Name = Detail.HeaderInfo.TitleEn,
                            ItemPageLink = this._link,
                            //ImageUri = Detail.HeaderInfo.HeaderImage,
                            ImageByte = await HttpHelper.GetByteArray(Detail.HeaderInfo.HeaderImage,SettingHelpers.GetSetting<string>("cookie"))
                        });
                        _favorState = true;
                    }
                    OnPropertyChanged("FavorIcon");
                    OnPropertyChanged("FavorButtonText");
                });
            }
        }

        public ICommand ReTryCommand
        {
            get
            {
                return new DelegateCommand(() =>
                {
                    isLoadFail = false;
                    OnLoaded(_link);
                });
            }
        }
        public ICommand ShareCommand
        {
            get
            {
                return new DelegateCommand(async () =>
                {
                    var imgbyte = await HttpHelper.GetByteArray(Detail.HeaderInfo.HeaderImage, SettingHelpers.GetSetting<string>("cookie") + ParseHelper.unconfig);
                    await ImageHelper.ShareImage(imgbyte, (Detail.HeaderInfo.TitleJp ?? Detail.HeaderInfo.TitleEn) + "------" + _link);
                });
            }
        }
        public ICommand ReadCommand
        {
            get
            {
                return new DelegateCommand(() =>
                {
                    App.rootFrame.Navigate(typeof(ReadingPage), new ReadingViewModel(this._link,this.Detail.HeaderInfo.TitleEn));
                });
            }
        }
        public ICommand DownLoadCommand
        {
            get
            {
                return new DelegateCommand(async () =>
                {
                    //var cachefolder = await ApplicationData.Current.LocalCacheFolder.CreateFolderAsync("download",CreationCollisionOption.OpenIfExists);
                    //var folder = await cachefolder.CreateFolderAsync(this.Detail.HeaderInfo.TitleEn.GetHashedString(), CreationCollisionOption.OpenIfExists);
                    //if (App.DownLoadList==null)
                    //{
                    //    App.DownLoadList = new List<DownLoadModel>();
                    //}
                    //App.DownLoadList.Add(new DownLoadModel(this.Link, folder, this.Detail.HeaderInfo.TitleEn));
                    MessageDialog dialog = new MessageDialog("Now Buliding", "Sorry");
                    await dialog.ShowAsync();
                });
            }
        }

        private PageListModel _selectedPage;

        public PageListModel SelectedPage
        {
            get { return _selectedPage; }
            set
            {
                _selectedPage = value;
                LoadMoreImage(value.Uri);
                OnPropertyChanged("SelectedPage");
            }
        }

        public SymbolIcon FavorIcon
        {
            get
            {
                return _favorState ? new SymbolIcon(Symbol.UnFavorite) : new SymbolIcon(Symbol.Favorite);
            }
        }
        public string FavorButtonText
        {
            get
            {
                return _favorState ? "UnFavorite" : "Favorite";
            }
        }


        private bool _isImageOnLoading;

        public bool isImageOnLoading
        {
            get { return _isImageOnLoading; }
            set { _isImageOnLoading = value; OnPropertyChanged("isImageOnLoading"); }
        }

        public List<PageListModel> PageList { get; set; }

        private ExHentaiLib.Prop.DetailProp _detail;
        public ExHentaiLib.Prop.DetailProp Detail
        {
            get { return _detail; }
            set { _detail = value; OnPropertyChanged("Detail"); }
        }

    }
}
