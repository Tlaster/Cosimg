using CosImg.Common;
using CosImg.CosImg.Common;
using CosImg.ExHentai.Model;
using CosImg.ExHentai.View;
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

namespace CosImg.ExHentai.ViewModel
{
    public class ExMainPageViewModel : LoadProps
    {
        private int _pivotSelectedIndex;

        public int PivotSelectedIndex
        {
            get { return _pivotSelectedIndex; }
            set { _pivotSelectedIndex = value; OnPropertyChanged("PivotSelectedIndex"); }
        }

        public ICommand RefreshCommand
        {
            get
            {
                return new DelegateCommand(() =>
                {
                    MainItemModel.OnLoaded("http://exhentai.org/?");
                });
            }
        }


        public PivotItemModel MainItemModel { get; set; }
        public SearchModel SearchItemModel { get; set; }

        public ExMainPageViewModel()
        {
            MainItemModel = new PivotItemModel("http://exhentai.org/?");
            SearchItemModel = new SearchModel();
        }

    }
}
