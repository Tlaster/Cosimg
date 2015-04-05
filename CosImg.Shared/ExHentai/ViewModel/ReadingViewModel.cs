using CosImg.ExHentai.Model;
using ExHentaiLib.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.Security.Cryptography;
using Windows.Security.Cryptography.Core;
using TBase;
using CosImg.Common;
using TBase.RT;
using Windows.Networking.Connectivity;
using CosImg.ExHentai.Common;
using System.Net.NetworkInformation;

namespace CosImg.ExHentai.ViewModel
{
    public class ReadingViewModel:LoadProps
    {
        private string _link;
        private List<ExHentaiLib.Prop.ImageListInfo> _pageList;
        private string _headerEn;
        private bool _isDownLoaded;
        private string _imageIndex;
        public bool isFlipBookView { get; set; }

        public ReadingViewModel(string link,string headerEn)
        {
            isFlipBookView = SettingHelpers.GetSetting<bool>("isFlipBookView");
            OnPropertyChanged("isFlipBookView");
            this._headerEn = headerEn;
            this._link = link;
            ImageList = new List<ImageModel>();
            OnLoaded();
        }
        public ReadingViewModel(string link, string headerEn, string imageIndex)
            : this(link, headerEn)
        {
            this._imageIndex = imageIndex;
        }

        private async void OnLoaded()
        {
            isOnLoading = true;
            var temp = new List<ImageModel>();
            if (await DownLoadDBHelpers.CheckItemisDownloaded(this._headerEn.GetHashedString()))
            {
                CurrentState = "Now Offline,checking download file";
                var a = await DownLoadDBHelpers.Query(_headerEn.GetHashedString());
                for (int i = 0; i < a.CurrentPage; i++)
                {
                    temp.Add(new ImageModel()
                    {
                        ImageIndex = i,
                        SaveFolder = _headerEn.GetHashedString(),
                        isDownLoaded = true,
                    });
                }
                await GetImageList();
            }
            else
            {
                if (!NetworkInterface.GetIsNetworkAvailable())
                {
                    CurrentState = "Now Offline,Load Failed";
                }
                else
                {
                    CurrentState = "Loading...";
                    this._pageList = await ParseHelper.GetImagePageListAsync(_link, SettingHelpers.GetSetting<string>("cookie"));
                    for (int i = 0; i < _pageList.Count; i++)
                    {
                        temp.Add(new ImageModel()
                        {
                            ImageIndex = i,
                            ImagePage = _pageList[i].ImagePage,
                            SaveFolder = _headerEn.GetHashedString(),
                            isDownLoaded = this._isDownLoaded,
                        });
                    }
                }
            }
            ImageList = temp;
            OnPropertyChanged("ImageList");

            if (_imageIndex != null && ImageList != null)
            {
                int numericalIndex = 0;
                try { numericalIndex = Convert.ToInt32(_imageIndex) - 1; } catch (Exception) { }
                if (numericalIndex < 0 || numericalIndex >= ImageList.Count()) numericalIndex = 0;
                SelectIndex = numericalIndex;
                OnPropertyChanged("SelectIndex");
            }
            isOnLoading = false;
        }

        private async Task GetImageList()
        {
            if (await DownLoadDBHelpers.CheckItemisDownloaded(this._headerEn.GetHashedString()) && NetworkInterface.GetIsNetworkAvailable())
            {
                Task.Run(async () =>
                {
                    this._pageList = await ParseHelper.GetImagePageListAsync(_link, SettingHelpers.GetSetting<string>("cookie"));
                    for (int i = 0; i < _pageList.Count; i++)
                    {
                        ImageList[i].ImagePage = _pageList[i].ImagePage;
                    }
                });
            }
        }



        public int SelectIndex { get; set; }

#if WINDOWS_PHONE_APP

        public ICommand RefreshCommand
        {
            get
            {
                return new DelegateCommand(async () =>
                {
                    await ImageList[SelectIndex].Refresh();
                });
            }
        }
        public ICommand SaveCommand
        {
            get
            {
                return new DelegateCommand(() =>
                {
                    ImageList[SelectIndex].Save();
                });
            }
        }

        public ICommand ShareCommand
        {
            get
            {
                return new DelegateCommand(() =>
                {
                    ImageList[SelectIndex].Share();
                });
            }
        }
#endif

        private string _currentState;

        public string CurrentState
        {
            get { return _currentState; }
            set { _currentState = value; OnPropertyChanged("CurrentState"); }
        }

        public List<ImageModel> ImageList { get; set; }
    }
}
