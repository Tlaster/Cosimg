using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using ExHentaiLib.Common;
using ExHentaiLib.Interface;
using TBase;

namespace ExHentaiViewer.WPF.ViewModel
{
    public class BaseViewModel : NotifyPropertyChanged, IGetCookie
    {

        public async void OnLoadError()
        {
            //I don't know how to begin a storyboard in ViewModel
            OnErrorBorderVisibility = Visibility.Visible;
            await Task.Delay(1200);
            OnErrorBorderVisibility = Visibility.Collapsed;
        }
        private Visibility _onErrorBorderVisibility = Visibility.Collapsed;
        public Visibility OnErrorBorderVisibility
        {
            get { return _onErrorBorderVisibility; }
            set
            {
                _onErrorBorderVisibility = value;
                OnPropertyChanged("OnErrorBorderVisibility");
            }
        }
        public string RequestUrl { get; set; }
        public string GetCookie()
        {
            return File.ReadAllText(AppDomain.CurrentDomain.BaseDirectory + "cookie.cookie") + ParseHelper.unconfig;
        }
        #region ProgressBarProp
        public Visibility ProgressBarVisibility
        {
            get
            {
                return isOnLoading ? Visibility.Visible : Visibility.Collapsed;
            }
        }
        public Visibility LoadMoreButtonVisibility
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

        private bool _isOnLoading = false;
        public bool isOnLoading
        {
            get { return _isOnLoading; }
            set
            {
                _isOnLoading = value;
                OnPropertyChanged("isOnLoading");
                OnPropertyChanged("ProgressBarVisibility");
                OnPropertyChanged("LoadMoreButtonVisibility");
                OnPropertyChanged("RefreshButtonVisibility");
            }
        }
        #endregion
    }
}
