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
    public class DownLoadViewModel:NotifyPropertyChanged
    {
        private List<DownLoadInfo> _completedList;

        public DownLoadViewModel()
        {
            OnLoaded();
        }

        public ICommand DeleteCommand
        {
            get
            {
                return new DelegateCommand(() =>
                {
                    for (int i = 0; i < RemoveList.Count; i++)
                    {
                        App.DownLoadList.Remove(RemoveList[i] as DownLoadModel);
                    }
                    OnPropertyChanged("DownLoadList");
                });
            }
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
            get
            {
                return App.DownLoadList;
            }
        }

        public List<object> RemoveList { get; set; }
    }
}
