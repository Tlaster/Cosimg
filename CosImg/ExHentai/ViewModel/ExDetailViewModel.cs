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

namespace CosImg.ExHentai.ViewModel
{
    public class ExDetailViewModel : LoadProps
    {
        private string Link;

        public ExDetailViewModel(string link)
        {
            this.Link = link;
            OnLoaded(link);
        }

        private async void OnLoaded(string link)
        {
            try
            {
                isOnLoading = true;
                Detail = await ParseHelper.GetDetailAsync(link, SettingHelpers.GetSetting<string>("cookie"));
                PageList = new List<PageListModel>();
                for (int i = 0; i < Detail.DetailPageCount; i++)
                {
                    PageList.Add(new PageListModel() { Page = (i + 1).ToString(), Uri = this.Link + "?p=" + i });
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

        public ICommand ReTryCommand
        {
            get
            {
                return new DelegateCommand(() =>
                {
                    isLoadFail = false;
                    OnLoaded(Link);
                });
            }
        }

        public ICommand ReadCommand
        {
            get
            {
                return new DelegateCommand(() =>
                {
                    App.rootFrame.Navigate(typeof(ReadingPage), this.Link);
                });
            }
        }
        public ICommand DownLoadCommand
        {
            get
            {
                return new DelegateCommand(() =>
                {

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
