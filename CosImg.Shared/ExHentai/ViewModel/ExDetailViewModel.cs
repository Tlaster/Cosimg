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
using ExHentaiLib.Prop;
using Windows.Networking.Connectivity;
using System.Net.NetworkInformation;
using Windows.UI.Xaml.Media.Imaging;
using Windows.ApplicationModel.DataTransfer;
using Windows.Storage.Streams;
using System.IO;

namespace CosImg.ExHentai.ViewModel
{
    public class ExDetailViewModel : LoadProps
    {
        private string _link;
        private bool _favorState;
        private byte[] _headerImageByte;

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
                    _headerImageByte = await HttpHelper.GetByteArrayWithPostMethod(Detail.HeaderInfo.HeaderImage, SettingHelpers.GetSetting<string>("cookie"));
                    _headerImage = await ImageHelper.ByteArrayToBitmapImage(_headerImageByte);
                    OnPropertyChanged("HeaderImage");
                }
                _favorState = _favoritem == null ? false : true;
                isDownLoaded = _downlaoditem != null && _downlaoditem.DownLoadComplete ? true : false;
                if (isDownLoaded && await FileHelpers.GetDetailCache(Detail.HeaderInfo.TitleEn.GetHashedString()) == null)
                {
                    await FileHelpers.SaveDetailCache(Detail.HeaderInfo.TitleEn.GetHashedString(), detailStr);
                } 
                RegisterForShare();
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
                return new DelegateCommand(() =>
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
                            ImageByte = _headerImageByte
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
        public ICommand ShareCommand
        {
            get
            {
                return new DelegateCommand(() =>
                {
                    DataTransferManager.ShowShareUI();
                });
            }
        }

        private void RegisterForShare()
        {
            DataTransferManager dataTransferManager = DataTransferManager.GetForCurrentView();
            dataTransferManager.DataRequested += ShareHandler;
        }

        private async void ShareHandler(DataTransferManager sender, DataRequestedEventArgs e)
        {
            if (_headerImageByte!=null)
            {
                DataRequestDeferral deferral = e.Request.GetDeferral();
                DataRequest request = e.Request;
                request.Data.Properties.Title = "This Hentai Item";
                request.Data.SetText((Detail.HeaderInfo.TitleJp == "" ? Detail.HeaderInfo.TitleEn : Detail.HeaderInfo.TitleJp) + "------" + _link);
                InMemoryRandomAccessStream randonAcc = new InMemoryRandomAccessStream();
                IOutputStream outputStream = randonAcc.GetOutputStreamAt(0);
                DataWriter writer = new DataWriter(outputStream);
                writer.WriteBytes(this._headerImageByte);
                await writer.StoreAsync();
                await writer.FlushAsync();
                RandomAccessStreamReference streamRef = RandomAccessStreamReference.CreateFromStream(randonAcc);
                request.Data.SetBitmap(streamRef);
                deferral.Complete();
            }
        }



        public ICommand ReadCommand
        {
            get
            {
                return new DelegateCommand(() =>
                {
                    App.rootFrame.Navigate(typeof(ReadingPage), new ReadingViewModel(this._link, this.Detail.HeaderInfo.TitleEn));

                });
            }
        }

        public ICommand ImageItemClick
        {
            get
            {
                return new DelegateCommand<ItemClickEventArgs>((e) =>
                {
                    var item = e.ClickedItem as ImageListInfo;
                    App.rootFrame.Navigate(typeof(ReadingPage), new ReadingViewModel(this._link, this.Detail.HeaderInfo.TitleEn, item.ImagePage));

                });
            }
        }

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
                        await new MessageDialog("Is Downloading").ShowAsync();
                    }
                    else
                    {

                        ToastPrompt toast = new ToastPrompt("Now Processing", true);
                        toast.ShowWithProgressBar();
                        await StartDownLoad();
                        toast.HideWithProgressBar();
                        new ToastPrompt("Downloading").Show();
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
            App.DownLoadList.Add(new DownLoadModel(this._link, this.Detail.HeaderInfo.TitleEn, _headerImageByte));
            if (!_favorState)
            {
                FavorDBHelpers.Add(new FavorModel()
                {
                    HashString = Detail.HeaderInfo.TitleEn.GetHashedString(),
                    Name = Detail.HeaderInfo.TitleEn,
                    ItemPageLink = this._link,
                    ImageByte = _headerImageByte
                });
            }
            DownLoadDBHelpers.Add(new DownLoadInfo()
            {
                PageUri = this._link,
                Name = Detail.HeaderInfo.TitleEn,
                HashString = Detail.HeaderInfo.TitleEn.GetHashedString(),
                Imagebyte = _headerImageByte
            });
            _favorState = true;
            OnPropertyChanged("FavorIcon");
            OnPropertyChanged("FavorButtonText");
        }


        

        private bool _isDownLoaded;

        public bool isDownLoaded
        {
            get { return _isDownLoaded; }
            set { _isDownLoaded = value; OnPropertyChanged("isDownLoaded"); }
        }
        private BitmapImage _headerImage;
        public BitmapImage HeaderImage
        {
            get
            {
                return _headerImage;
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
        private FavorModel _favoritem;
        private DownLoadInfo _downlaoditem;
        public ExHentaiLib.Prop.DetailProp Detail
        {
            get { return _detail; }
            set { _detail = value; OnPropertyChanged("Detail"); }
        }
    }


}
