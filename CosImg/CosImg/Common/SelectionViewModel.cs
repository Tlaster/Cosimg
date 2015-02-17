using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using TBase;
using Windows.Data.Json;
using Windows.Graphics.Display;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace CosImg.CosImg.Common
{
    public class SelectionViewModel<T> :BaseViewModel<T>
    {
        private Func<IJsonValue, T> _generator;


        public SelectionViewModel(string link,Func<IJsonValue, T> generator)
        {
            Link = link;
            _generator = generator;
            OnLoaded();
        }


        public void OnLoaded()
        {
            List = new GeneratorIncrementalLoadingClass<T>(Link, _generator);
            List.OnLoading += (s, e) => { isOnLoading = true; };
            List.LoadFailed += (s, e) => { isLoadFail = true; isOnLoading = false; };
            List.LoadSucceed += (s, e) => { isOnLoading = false; };
        }


        public override System.Windows.Input.ICommand ReTryCommand
        {
            get
            {
                return new DelegateCommand(() =>
                {
                    isLoadFail = false; 
                    OnLoaded();
                });
            }
        }

        public override System.Windows.Input.ICommand ItemTapped
        {
            get
            {
                return new DelegateCommand<ItemClickEventArgs>((item) =>
                {
                    App.rootFrame.Navigate(typeof(CosImg.View.ImagePage), (T)item.ClickedItem);
                });
            }
        }

    }
}
