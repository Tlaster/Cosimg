using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows;
using TBase;

namespace ExHentaiViewer.WPF.ViewModel
{
    public class ProgressInfo : NotifyPropertyChanged
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
                return isOnLoading ? Visibility.Collapsed : Visibility.Visible;
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

    }
}
