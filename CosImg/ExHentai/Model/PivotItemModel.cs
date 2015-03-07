using CosImg.Common;
using CosImg.CosImg.Common;
using CosImg.ExHentai.View;
using CosImg.ExHentai.ViewModel;
using ExHentaiLib.Common;
using ExHentaiLib.Prop;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using TBase;
using TBase.RT;
using Windows.UI.Xaml.Controls;

namespace CosImg.ExHentai.Model
{
    public class PivotItemModel : LoadProps
    {
        private ObservableCollection<MainListProp> _list;
        public string Link;

        public ObservableCollection<MainListProp> List
        {
            get { return _list; }
            set { _list = value; OnPropertyChanged("List"); }
        }

        

        public PivotItemModel() { }

        public PivotItemModel(string link)
        {
            Link = link;
            OnLoaded(Link);
        }

        public async void OnLoaded(string uri)
        {
            try
            {
                isOnLoading = true;
                List = await ParseHelper.GetMainListAsync(uri, SettingHelpers.GetSetting<string>("cookie"));
                isOnLoading = false;
            }
            catch (Exception)
            {
                isLoadFail = true;
                isOnLoading = false;
            }
        }

        public async void LoadMore(string uri)
        {
            try
            {
                isOnLoading = true;
                var morelist = await ParseHelper.GetMainListAsync(uri, SettingHelpers.GetSetting<string>("cookie"));
                for (int i = 0; i < morelist.Count; i++)
                {
                    List.Add(morelist[i]);
                }
                isOnLoading = false;
            }
            catch (Exception)
            {
                isOnLoading = false;
                new ToastPrompt("Load Failed").Show();
            }
        }


        public System.Windows.Input.ICommand LoadMoreCommand
        {
            get
            {
                return new DelegateCommand(() =>
                {
                    if (List.Count % 25 != 0)
                    {
                        return;
                    }
                    else
                    {
                        var a = List.Count / 25;
                        LoadMore(Link + "&page=" + (a).ToString());
                    }
                });
            }
        }






        public System.Windows.Input.ICommand ReTryCommand
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

        public ICommand Refresh
        {
            get
            {
                return new DelegateCommand(() =>
                {
                    List = new System.Collections.ObjectModel.ObservableCollection<MainListProp>();
                    OnLoaded(Link);
                });
            }
        }

        public System.Windows.Input.ICommand ItemClick
        {
            get
            {
                return new DelegateCommand<ItemClickEventArgs>((e) =>
                {
                    App.rootFrame.Navigate(typeof(ExDetailPage), new ExDetailViewModel((e.ClickedItem as MainListProp).Link));
                });
            }
        }
    }
}
