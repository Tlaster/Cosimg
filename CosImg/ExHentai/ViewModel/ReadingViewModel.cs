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
    public class ReadingViewModel:TBase.NotifyPropertyChanged
    {
        private string _link;
        private List<ExHentaiLib.Prop.ImageListInfo> _pageList;
        private string _headerEn;
        private string _imagePage;
        private bool _isDownLoaded;
        public ReadingViewModel(string link,string headerEn,bool isDownLoaded = false)
        {
            this._headerEn = headerEn;
            this._link = link;
            this._isDownLoaded = isDownLoaded;
            ImageList = new List<ImageModel>();
            OnLoaded();
        }

        public ReadingViewModel(string link, string headerEn, string imagePage, bool isDownLoaded = false)
            : this(link, headerEn, isDownLoaded)
        {
            this._imagePage = imagePage;
        }


        private async void OnLoaded()
        {
            isOnLoading = true;
            var temp = new List<ImageModel>();
            if (!NetworkInterface.GetIsNetworkAvailable())
            {
                if (_isDownLoaded)
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
                }
                else
                {
                    CurrentState = "Now Offline,Load Failed";
                }
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
            ImageList = temp;
            OnPropertyChanged("ImageList");
            if (_imagePage != null)
            {
                SelectIndex = _pageList.FindIndex((a) => { return a.ImagePage == _imagePage; });
                OnPropertyChanged("SelectIndex");
            }
            isOnLoading = false;
        }


        public int SelectIndex { get; set; }
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


        private bool _isOnLoading;

        public bool isOnLoading
        {
            get { return _isOnLoading; }
            set { _isOnLoading = value; OnPropertyChanged("isOnLoading"); }
        }

        private string _currentState;

        public string CurrentState
        {
            get { return _currentState; }
            set { _currentState = value; OnPropertyChanged("CurrentState"); }
        }

        public List<ImageModel> ImageList { get; set; }
    }
}
