using CosImg.ExHentai.Common;
using CosImg.ExHentai.Model;
using CosImg.ExHentai.View;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using TBase;
using Windows.UI.Xaml.Controls;

namespace CosImg.ExHentai.ViewModel
{
    public class DownLoadViewModel:TBase.NotifyPropertyChanged
    {
        private List<DownLoadInfo> _completedList;

        public DownLoadViewModel()
        {
            OnLoaded();
        }

        public ICommand ItemClick
        {
            get
            {
                return new DelegateCommand<ItemClickEventArgs>((e) =>
                {
                    App.rootFrame.Navigate(typeof(ExDetailPage), new ExDetailViewModel((e.ClickedItem as DownLoadInfo).PageUri));
                });
            }
        }


        private async void OnLoaded()
        {
            _completedList = await DownLoadDBHelpers.GetList(true);
            OnPropertyChanged("CompletedList");
        }

        public List<DownLoadInfo> CompletedList
        {
            get
            {
                return _completedList;
            }
        }
        public List<DownLoadModel> DownLoadList
        {
            set
            {
                App.DownLoadList = value;
            }
            get
            {
                return App.DownLoadList;
            }
        }
    }
}
