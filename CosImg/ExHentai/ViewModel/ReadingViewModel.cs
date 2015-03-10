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

namespace CosImg.ExHentai.ViewModel
{
    public class ReadingViewModel:TBase.NotifyPropertyChanged
    {
        private string Link;
        private List<ExHentaiLib.Prop.ImageListInfo> PageList;
        private string HeaderEn;
        private string _imagePage;
        private bool _isDownLoaded;
        public ReadingViewModel(string link,string headerEn,bool isDownLoaded = false)
        {
            this.HeaderEn = headerEn;
            this.Link = link;
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
            this.PageList = await ParseHelper.GetImagePageListAsync(Link, SettingHelpers.GetSetting<string>("cookie"));
            var temp = new List<ImageModel>();
            for (int i = 0; i < PageList.Count; i++)
            {
                temp.Add(new ImageModel() 
                {
                    ImageIndex = i,
                    ImagePage = PageList[i].ImagePage,
                    SaveFolder = HeaderEn.GetHashedString(),
                    isDownLoaded = this._isDownLoaded,
                });
            }
            ImageList = temp;
            OnPropertyChanged("ImageList");
            if (_imagePage != null)
            {
                SelectIndex = PageList.FindIndex((a) => { return a.ImagePage == _imagePage; });
                OnPropertyChanged("SelectIndex");
            }
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

        public List<ImageModel> ImageList { get; set; }
    }
}
