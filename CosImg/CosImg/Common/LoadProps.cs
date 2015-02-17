using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TBase;
using Windows.UI.Xaml;

namespace CosImg.CosImg.Common
{
    public abstract class LoadProps : NotifyPropertyChanged
    {
        private bool _isOnLoading;

        public bool isOnLoading
        {
            get { return _isOnLoading; }
            set
            {
                _isOnLoading = value;
                OnPropertyChanged("isOnLoading");
                OnPropertyChanged("ProVis");
                OnPropertyChanged("LoadMoreVis");
            }
        }

        private bool _isLoadFail = false;

        public bool isLoadFail
        {
            get { return _isLoadFail; }
            set
            {
                _isLoadFail = value; OnPropertyChanged("FailVis");
                OnPropertyChanged("LoadMoreVis");
            }
        }

        public Visibility FailVis
        {
            get
            {
                return isLoadFail ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        public virtual Visibility LoadMoreVis
        {
            get
            {
                return !isOnLoading && !isLoadFail ? Visibility.Visible : Visibility.Collapsed;
            }
        }


        public Visibility ProVis
        {
            get
            {
                return isOnLoading ? Visibility.Visible : Visibility.Collapsed;
            }
        }
    }
}
