using System.ComponentModel;
using System.Windows;

namespace ExHentaiViewer.WPF.ViewModel
{
    public class ProgressInfo : INotifyPropertyChanged
    {
        public Visibility ProgressBarVisibility
        {
            get
            {
                if (isOnLoading)
                {
                    return Visibility.Visible;
                }
                else
                {
                    return Visibility.Collapsed;
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
            }
        }



        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = this.PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion

    }
}
