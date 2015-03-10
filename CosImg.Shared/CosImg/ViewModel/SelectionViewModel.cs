using CosImg.CosImg.Model;
using CosImg.CosImg.View;
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
    public class SelectionViewModel : BaseViewModel<ListModel>
    {
        private Func<IJsonValue, ListModel> _generator;
        public List<SelectionModel> ModelList { get; set; }


        public SelectionViewModel(string link, Func<IJsonValue, ListModel> generator, List<SelectionModel> list = null)
        {
            _link = link;
            _generator = generator;
            ModelList = list;
            OnLoaded();
        }


        public void OnLoaded()
        {
            List = new GeneratorIncrementalLoadingClass<ListModel>(_link, _generator);
            List.OnLoading += (s, e) => { isOnLoading = true; };
            List.LoadFailed += (s, e) => { isLoadFail = true; isOnLoading = false; };
            List.LoadSucceed += (s, e) => { isOnLoading = false; };
        }

        public ICommand ModelPicked
        {
            get
            {
                return new DelegateCommand<SelectionModel>((item) =>

                {
                    this._link = item.Link;
                    this._generator = item.Generator;
                    OnLoaded();
                });
            }
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
                return new DelegateCommand<ItemClickEventArgs>((e) =>
                {
#if WINDOWS_PHONE_APP
                    App.rootFrame.Navigate(typeof(CosImg.View.ImagePage), e.ClickedItem as ListModel);
#else
                    var item = e.ClickedItem as ListModel;
                    new ImagePopUp(item).Show();
#endif
                });
            }
        }

    }
}
