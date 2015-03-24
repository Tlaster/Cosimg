﻿using CosImg.CosImg.Common;
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
using ExHentaiLib.Prop;
using Windows.Networking.Connectivity;
using System.Net.NetworkInformation;

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
            isOnLoading = true;
            string detailStr = "";
            try
            {
                if (!NetworkInterface.GetIsNetworkAvailable())
                {
                    _downlaoditem = await DownLoadDBHelpers.QueryFromLink(link);
                    _favoritem = await FavorDBHelpers.QueryFromLink(link);
                    if (_downlaoditem == null)
                    {
                        new ToastPrompt("Now Offline").Show();
                        throw new KeyNotFoundException();
                    }
                    else
                    {
                        detailStr = await FileHelpers.GetDetailCache(_downlaoditem.HashString);
                        if (detailStr != null)
                        {
                            Detail = ParseHelper.GetDetailFromString(detailStr);
                            new ToastPrompt("Now Offline,Loading Cache").Show();
                        }
                        else
                        {
                            new ToastPrompt("Now Offline").Show();
                            throw new KeyNotFoundException();
                        }
                    }
                }
                else
                {
                    detailStr = await HttpHelper.GetStringWithCookie(_link, SettingHelpers.GetSetting<string>("cookie") + ParseHelper.unconfig);
                    Detail = ParseHelper.GetDetailFromString(detailStr);
                    _favoritem = await FavorDBHelpers.Query(Detail.HeaderInfo.TitleEn.GetHashedString());
                    _downlaoditem = await DownLoadDBHelpers.Query(Detail.HeaderInfo.TitleEn.GetHashedString());
                    PageList = new List<PageListModel>();
                    for (int i = 0; i < Detail.DetailPageCount; i++)
                    {
                        PageList.Add(new PageListModel() { Page = (i + 1).ToString(), Uri = this._link + "?p=" + i });
                    }
                }
                _favorState = _favoritem == null ? false : true;
                isDownLoaded = _downlaoditem != null && _downlaoditem.DownLoadComplete ? true : false;
                if (isDownLoaded && await FileHelpers.GetDetailCache(Detail.HeaderInfo.TitleEn.GetHashedString()) == null)
                {
                    await FileHelpers.SaveDetailCache(Detail.HeaderInfo.TitleEn.GetHashedString(), detailStr);
                }

            }
            catch (Exception)
            {
                isLoadFail = true;
            }
            isOnLoading = false;
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
                        ToastPrompt toast = new ToastPrompt("Now Processing", true);
                        toast.ShowWithProgressBar();
                        FavorDBHelpers.Add(new FavorModel() 
                        { 
                            HashString = Detail.HeaderInfo.TitleEn.GetHashedString(),
                            Name = Detail.HeaderInfo.TitleEn,
                            ItemPageLink = this._link,
                            ImageByte = await HttpHelper.GetByteArrayWithPostMethod(Detail.HeaderInfo.HeaderImage,SettingHelpers.GetSetting<string>("cookie"))
                        });
                        _favorState = true;
                        toast.HideWithProgressBar();
                        new ToastPrompt("Succeed").Show();
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
#if WINDOWS_PHONE_APP
        public ICommand ShareCommand
        {
            get
            {
                return new DelegateCommand(async () =>
                {
                    var imgbyte = await HttpHelper.GetByteArray(Detail.HeaderInfo.HeaderImage, SettingHelpers.GetSetting<string>("cookie") + ParseHelper.unconfig);

                    await ImageHelper.ShareImage(imgbyte, (Detail.HeaderInfo.TitleJp == "" ? Detail.HeaderInfo.TitleEn : Detail.HeaderInfo.TitleJp) + "------" + _link);
                });
            }
        }
#endif

        public ICommand ReadCommand
        {
            get
            {
                return new DelegateCommand(() =>
                {
                    App.rootFrame.Navigate(typeof(ReadingPage), new ReadingViewModel(this._link, this.Detail.HeaderInfo.TitleEn, isDownLoaded));

                });
            }
        }

#if WINDOWS_PHONE_APP
        public ICommand ImageItemClick
        {
            get
            {
                return new DelegateCommand<ItemClickEventArgs>((e) =>
                {
                    var item = e.ClickedItem as ImageListInfo;
                    App.rootFrame.Navigate(typeof(ReadingPage), new ReadingViewModel(this._link, this.Detail.HeaderInfo.TitleEn, item.ImagePage, isDownLoaded));

                });
            }
        }
#endif

        public ICommand DeleFileCommand
        {
            get
            {
                return new DelegateCommand(async () =>
                {
                    MessageDialog dialog = new MessageDialog("Delete the Download File?", "Sure?");
                    dialog.Commands.Add(new UICommand("Yes", async (e1) =>
                    {
                        DownLoadDBHelpers.Delete(Detail.HeaderInfo.TitleEn.GetHashedString());
                        await ImageHelper.DeleDownloadFile(Detail.HeaderInfo.TitleEn.GetHashedString());
                        isDownLoaded = false;
                        OnPropertyChanged("isDownLoaded"); 
                        new ToastPrompt("Delete Complete").Show();
                    }));
                    dialog.Commands.Add(new UICommand("No"));
                    await dialog.ShowAsync();
                });
            }
        }

        public ICommand DownLoadCommand
        {
            get
            {
                return new DelegateCommand(async () =>
                {
                    if (App.DownLoadList != null && App.DownLoadList.Find((a) => { return a.HashString == Detail.HeaderInfo.TitleEn.GetHashedString(); }) != null)
                    {
                        MessageDialog dialog = new MessageDialog("Is Downloading");
                        await dialog.ShowAsync();
                    }
                    else
                    {

                        ToastPrompt toast = new ToastPrompt("Now Processing", true);
                        toast.ShowWithProgressBar();
                        await StartDownLoad();
                        toast.HideWithProgressBar();
                    }
                });
            }
        }

        private async Task StartDownLoad()
        {
            if (App.DownLoadList == null)
            {
                App.DownLoadList = new List<DownLoadModel>();
            }
            await FileHelpers.SaveDetailCache(Detail.HeaderInfo.TitleEn.GetHashedString(), await HttpHelper.GetStringWithCookie(_link, SettingHelpers.GetSetting<string>("cookie") + ParseHelper.unconfig));
            var imgbyte = await HttpHelper.GetByteArrayWithPostMethod(Detail.HeaderInfo.HeaderImage, SettingHelpers.GetSetting<string>("cookie"));
            App.DownLoadList.Add(new DownLoadModel(this._link, this.Detail.HeaderInfo.TitleEn, imgbyte));
            if (!_favorState)
            {
                FavorDBHelpers.Add(new FavorModel()
                {
                    HashString = Detail.HeaderInfo.TitleEn.GetHashedString(),
                    Name = Detail.HeaderInfo.TitleEn,
                    ItemPageLink = this._link,
                    ImageByte = imgbyte
                });
            }
            DownLoadDBHelpers.Add(new DownLoadInfo()
            {
                PageUri = this._link,
                Name = Detail.HeaderInfo.TitleEn,
                HashString = Detail.HeaderInfo.TitleEn.GetHashedString(),
                Imagebyte = imgbyte
            });
            _favorState = true;
            new ToastPrompt("Downloading").Show();
            OnPropertyChanged("FavorIcon");
            OnPropertyChanged("FavorButtonText");
        }


        private bool _isDownLoaded;

        public bool isDownLoaded
        {
            get { return _isDownLoaded; }
            set { _isDownLoaded = value; OnPropertyChanged("isDownLoaded"); }
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
        private FavorModel _favoritem;
        private DownLoadInfo _downlaoditem;
        public ExHentaiLib.Prop.DetailProp Detail
        {
            get { return _detail; }
            set { _detail = value; OnPropertyChanged("Detail"); }
        }
    }


}
