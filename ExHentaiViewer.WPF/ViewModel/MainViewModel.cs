using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using ExHentaiLib.Common;
using ExHentaiLib.Prop;
using TBase;
using ExHentaiViewer.WPF.Common;

namespace ExHentaiViewer.WPF.ViewModel
{
    public class MainViewModel : BaseViewModel
    {
        public MainViewModel()
        {
            RequestUrl = "http://exhentai.org/?";
            OnLoaded(RequestUrl, CookieHelper.GetCookie());
        }


        private async void OnLoaded(string uri,string cookie)
        {
            try
            {
                isOnLoading = true;
                MainList = await ParseHelper.GetMainListAsync(uri, cookie);
                isOnLoading = false;
            }
            catch (Exception)
            {
                isOnLoading = false;
                OnLoadError();
            }
        }

        private async void LoadMore(string uri,string cookie)
        {
            try
            {
                isOnLoading = true;
                var morelist = await ParseHelper.GetMainListAsync(uri,cookie);
                for (int i = 0; i < morelist.Count; i++)
                {
                    MainList.Add(morelist[i]);
                }
                isOnLoading = false;
            }
            catch (Exception)
            {
                isOnLoading = false;
                OnLoadError();
            }
        }

        public ICommand LoadMoreList
        {
            get
            {
                return new DelegateCommand(() =>
                {
                    var a = MainList.Count / 25;
                    LoadMore(RequestUrl + "&page=" + (a).ToString(), CookieHelper.GetCookie());
                });
            }
        }

        public ICommand Refresh
        {
            get
            {
                return new DelegateCommand(() =>
                {
                    OnLoaded(RequestUrl, CookieHelper.GetCookie());
                });
            }
        }

        public ICommand SearchClick
        {
            get
            {
                return new DelegateCommand((searchStr) =>
                {
                    RequestUrl = "http://exhentai.org/?f_doujinshi=1&f_manga=1&f_artistcg=1&f_gamecg=1&f_western=1&f_non-h=1&f_imageset=1&f_cosplay=1&f_asianporn=1&f_misc=1&f_search=" + searchStr + "&f_apply=Apply+Filter";
                    OnLoaded(RequestUrl, CookieHelper.GetCookie());
                });
            }
        }
        public ICommand ListItemClick
        {
            get
            {
                return new DelegateCommand((selectItem) =>
                {
                    var item = selectItem as MainListProp;
                    new DetailWindow(item.Link).Show();
                });
            }
        }


        public Visibility RefreshButtonVisibility
        {
            get
            {
                if (isOnLoading)
                {
                    return Visibility.Collapsed;
                }
                else
                {
                    return Visibility.Visible;
                }
            }
        }




        private ObservableCollection<MainListProp> _mainList;
        public ObservableCollection<MainListProp> MainList
        {
            get { return _mainList; }
            set
            {
                _mainList = value;
                OnPropertyChanged("MainList");
            }
        }


    }
}
