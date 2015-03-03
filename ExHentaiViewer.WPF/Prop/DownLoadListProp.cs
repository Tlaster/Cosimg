using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using ExHentaiLib.Common;
using ExHentaiLib.Prop;
using ExHentaiViewer.WPF.Common;
using ExHentaiViewer.WPF.ViewModel;
using TBase;

namespace ExHentaiViewer.WPF.Prop
{
    public class DownLoadListProp : NotifyPropertyChanged
    {
        public DownLoadListProp(string uri,string savePath,string itemName)
        {
            ItemName = itemName;
            this._savePath = savePath;
            System.IO.Directory.CreateDirectory(_savePath);
            _pageUri = uri;
            _imagePageUri = new List<ImageListInfo>();
            webClient = new WebClient();
            webClient.DownloadProgressChanged += webClient_DownloadProgressChanged;
            webClient.Headers["Cookie"] = CookieHelper.GetCookie();
            webClient.DownloadDataCompleted += webClient_DownloadDataCompleted;
            StartDownLoading();
        }


        private async Task GetImagePageListAsync()
        {
            CurrentState = "Getting Image Count";
            ItemInfo = await ParseHelper.GetDetailAsync(_pageUri, CookieHelper.GetCookie());
            _imagePageUri = await ParseHelper.GetImagePageListAsync(_pageUri, CookieHelper.GetCookie());
            MaxImageCount = ItemInfo.MaxImageCount;
        }
        private async void StartDownLoading()
        {
            try
            {
                await GetImagePageListAsync();
                await DownloadFromUriList();
            }
            catch (Exception)
            {
                OnParse = true;
                CurrentState = "Error";
            }
        }
        private async Task DownloadFromUriList()
        {
            CurrentState = "Downloading...";
            try
            {
                _sourceUri = await ParseHelper.GetImageAync(_imagePageUri[CurrentDownLoadImage].ImagePage, CookieHelper.GetCookie());
                webClient.DownloadDataAsync(new Uri(_sourceUri));
            }
            catch (Exception)
            {
                OnParse = true;
                CurrentState = "Error";
            }
        }

        async void webClient_DownloadDataCompleted(object sender, DownloadDataCompletedEventArgs e)
        {
            if (!e.Cancelled)
            {
                if (webClient.ResponseHeaders["Content-Disposition"] != null)
                {
                    await DownloadFromUriList();
                }
                else
                {
                    File.WriteAllBytes(_savePath + "\\" + _imagePageUri[CurrentDownLoadImage].ImageName + System.IO.Path.GetExtension(_sourceUri), e.Result);
                    webClient.Dispose();
                    CurrentDownLoadImage += 1;
                    DownLoadProgress = 0;
                    if (CurrentDownLoadImage < MaxImageCount)
                    {
                        await DownloadFromUriList();
                    }
                    else
                    {
                        CurrentState = "Completed";
                        DownLoadProgress = 100;
                        webClient.Dispose();
                    }
                }
            }
        }


        private void webClient_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            DownLoadProgress = e.ProgressPercentage;
        }







        private string _pageUri;
        private string _savePath;
        private List<ImageListInfo> _imagePageUri;
        public WebClient webClient;

        private bool _onParse = false;

        public bool OnParse
        {
            get { return _onParse; }
            set 
            {
                _onParse = value;
                if (!value)
                {
                    DownloadFromUriList();
                }
                else
                {
                    this.webClient.CancelAsync();
                    CurrentState = "Parse";
                    DownLoadProgress = 0;
                }
            }
        }

        private int _downLoadProgress;

        public int DownLoadProgress
        {
            get { return _downLoadProgress; }
            set { _downLoadProgress = value; OnPropertyChanged("DownLoadProgress"); }
        }


        private int _courrentDownLoadImage = 0;
        public int CurrentDownLoadImage
        {
            get { return _courrentDownLoadImage; }
            set 
            {
                _courrentDownLoadImage = value;
                OnPropertyChanged("CurrentDownLoadImage");
            }
        }

        private int _maxImageCount;
        public int MaxImageCount
        {
            get { return _maxImageCount; }
            set
            {
                _maxImageCount = value;
                OnPropertyChanged("MaxImageCount");
            }
        }
        private string _currentState;
        private string _sourceUri;

        public string CurrentState
        {
            get { return _currentState; }
            set { _currentState = value; OnPropertyChanged("CurrentState"); }
        }
        public DetailProp ItemInfo { get; set; }
        public string ItemName { get; set; }
    }
}
