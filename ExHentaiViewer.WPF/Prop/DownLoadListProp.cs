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
        public string ItemName { get; set; }
        public DownLoadListProp(string uri,string savePath,string itemName)
        {
            ItemName = itemName;
            this.SavePath = savePath;
            System.IO.Directory.CreateDirectory(SavePath);
            PageUri = uri;
            ImagePageUri = new List<ImageListInfo>();
            StartDownLoading();
        }
        private bool _onParse = false;

        public bool OnParse
        {
            get { return _onParse; }
            set 
            {
                _onParse = value;
                if (!value)
                {
                    DownloadFromUriList(ImagePageUri);
                }
            }
        }

        private List<ImageListInfo> ImagePageUri;
        private async Task GetImagePageListAsync(string uri,string cookie)
        {
            CurrentState = "Getting Image Count";
            ItemInfo = await ParseHelper.GetDetailAsync(uri, cookie);
            ImagePageUri = await ParseHelper.GetImagePageListAsync(uri, cookie);
            MaxImageCount = ParseHelper.GetMaxImageCount(ItemInfo.ImageCountString);
        }
        private async void StartDownLoading()
        {
            try
            {
                await GetImagePageListAsync(PageUri, GetCookie());
                DownloadFromUriList(ImagePageUri);
            }
            catch (Exception)
            {
                OnParse = true;
            }
        }
        public string SavePath { get; set; }
        private void DownloadFromUriList(List<ImageListInfo> uriList)
        {
            CurrentState = "Downloading...";
            DownLoadFromUri(uriList[CurrentDownLoadImage].ImagePage, uriList[CurrentDownLoadImage].ImageName);
        }
        private string _currentState;

        public string CurrentState
        {
            get { return _currentState; }
            set { _currentState = value; OnPropertyChanged("CurrentState"); }
        }
        private void DownLoadFromUri(string uri,string fileName)
        {
            try
            {
                DownLoadHelper DH = new DownLoadHelper(uri, SavePath + "\\" + fileName, GetCookie());
                DH.DownLoadCompleted += DH_DownLoadCompleted;
                DH.DownLoadFailed += DH_DownLoadFailed;
            }
            catch (Exception)
            {
                OnParse = true;
            }
        }

        void DH_DownLoadFailed(object sender, EventArgs e)
        {
            OnParse = true;
            CurrentState = "Parse";
        }

        void DH_DownLoadCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {

            CurrentDownLoadImage += 1;
            if (OnParse)
            {
                CurrentState = "Parse";
                return;
            }
            if (CurrentDownLoadImage < MaxImageCount)
            {
                DownLoadFromUri(ImagePageUri[CurrentDownLoadImage].ImagePage, ImagePageUri[CurrentDownLoadImage].ImageName);
            }
            else
            {
                CurrentState = "Completed";
            }
        }



        private string GetCookie()
        {
            return File.ReadAllText(AppDomain.CurrentDomain.BaseDirectory + "cookie.cookie") + ParseHelper.unconfig;
        }
        private string PageUri { get; set; }
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

        public DetailProp ItemInfo { get; set; }
    }
}
