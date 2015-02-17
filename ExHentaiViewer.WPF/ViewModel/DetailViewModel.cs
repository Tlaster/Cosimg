using System;
using System.Threading.Tasks;
using System.Windows.Input;
using ExHentaiLib.Common;
using ExHentaiLib.Prop;
using TBase;

namespace ExHentaiViewer.WPF.ViewModel
{
    public class DetailViewModel : BaseViewModel
    {
        public DetailViewModel(string uri)
        {
            RequestUrl = uri;
            OnLoaded(RequestUrl, GetCookie());
        }

        private async void OnLoaded(string uri, string cookie)
        {
            try
            {
                isOnLoading = true;
                Detail = await ParseHelper.GetDetailAsync(uri, cookie);
                isOnLoading = false;
            }
            catch (Exception)
            {
                isOnLoading = false;
                OnLoadError();
            }
        }

        private async void LoadMoreImage(string uri, string cookie)
        {
            try
            {
                isOnLoading = true;
                var a = await ParseHelper.GetDetailAsync(uri, cookie);
                Detail.ImageList = a.ImageList;
                OnPropertyChanged("Detail");
                isOnLoading = false;
            }
            catch (Exception)
            {
                isOnLoading = false;
                OnLoadError();
            }
        }

        public ICommand BeginDownLoad
        {
            get
            {
                return new DelegateCommand(() =>
                {
                    App.downLoadWindow.Show();
                    App.downLoadWindow.AddDownload(RequestUrl, Detail);
                });
            }
        }
        public ICommand BeginReading
        {
            get
            {
                return new DelegateCommand(() =>
                {
                    new ReadingPage(RequestUrl).Show();
                });
            }
        }

        public ICommand ClickToRead
        {
            get
            {
                return new DelegateCommand((selectItem) =>
                {
                    var item = (ImageListInfo)selectItem;
                    new ReadingPage(RequestUrl, item).Show();
                });
            }
        }

        public ICommand LoadMoreList
        {
            get
            {
                return new DelegateCommand(() =>
                {
                    var a = SelectPage + 1;
                    LoadMoreImage(RequestUrl + "?p=" + a.ToString(), GetCookie());
                });
            }
        }

        private int _selectPage;
        public int SelectPage
        {
            get 
            {
                return _selectPage;
            }
            set 
            {
                _selectPage = value; 
                OnPropertyChanged("SelectPage"); 
            }
        }

        private DetailProp _detail;

        public DetailProp Detail
        {
            get { return _detail; }
            set
            { 
                _detail = value;
                OnPropertyChanged("Detail");
            }
        }



    }
}
