using CosImg.CosImg.Common;
using CosImg.CosImg.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using TBase;
using Windows.Data.Json;

namespace CosImg.CosImg.ViewModel
{
    public class MainViewModel :TBase.NotifyPropertyChanged
    {

        private static readonly Func<IJsonValue, ListModel> generator = (item) =>
        {
            return new ListModel()
            {
                image = item.GetObject()["photo"].GetObject()["sq300_url"].GetString(),
                name = item.GetObject()["photo"].GetObject()["subject"].GetString(),
                url = item.GetObject()["photo"].GetObject()["url"].GetString(),
                id = item.GetObject()["photo"].GetObject()["id"].GetNumber(),
            };
        };
        private static readonly Func<IJsonValue, ListModel> generator2 = (item) =>
        {
            return new ListModel()
            {
                image = item.GetObject()["sq300_url"].GetString(),
                //name = item.GetObject()["photo"].GetObject()["subject"].GetString(),
                url = item.GetObject()["url"].GetString(),
                id = item.GetObject()["id"].GetNumber(),
            };
        };

        private List<SelectionModel> _newList = new List<SelectionModel>()
        { 
            new SelectionModel() { Name = "最新照片", Link = "http://worldcosplay.net/api/photo/list?",Generator = generator }, 
            new SelectionModel() { Name = "最新随拍", Link = "http://worldcosplay.net/api/instants/list?",Generator = generator2 }
        };


        private List<SelectionModel> _popList = new List<SelectionModel>()
        {
            new SelectionModel(){ Name="按日统计",Link="http://worldcosplay.net/api/ranking/good?sort=daily_good_cnt",Generator=generator},
            new SelectionModel(){ Name="按周统计",Link="http://worldcosplay.net/api/ranking/good?sort=weekly_good_cnt",Generator=generator},
            new SelectionModel(){ Name="按月统计",Link="http://worldcosplay.net/api/ranking/good?sort=monthly_good_cnt",Generator=generator},
            new SelectionModel(){ Name="累计",Link="http://worldcosplay.net/api/ranking/good?sort=good_cnt",Generator=generator},
        };




        public MainViewModel()
        {
            FoundViewModel = new SelectionViewModel("http://worldcosplay.net/api/photo/popular?", generator);
            NewViewModel = new SelectionViewModel("http://worldcosplay.net/api/photo/list?", generator, _newList);
            PopViewModel = new SelectionViewModel("http://worldcosplay.net/api/ranking/good?sort=daily_good_cnt", generator, _popList);
#if WINDOWS_PHONE_APP
            SearchViewModel = new SearchModel<ListModel>(generator);
#endif
        }

#if WINDOWS_PHONE_APP
        public SearchModel<ListModel> SearchViewModel { get; set; }
#endif


        public ICommand RefreshCommand
        {
            get
            {
                return new DelegateCommand(() =>
                {
                    switch (SelectedIndex)
                    {
                        case 0:
                            FoundViewModel.OnLoaded();
                            break;
                        case 1: 
                            NewViewModel.OnLoaded();
                            break;
                        case 2: 
                            PopViewModel.OnLoaded();
                            break;
                        default:
                            break;
                    }
                });
            }
        }

        private SelectionViewModel _popViewModel;

        public SelectionViewModel PopViewModel
        {
            get { return _popViewModel; }
            private set { _popViewModel = value; OnPropertyChanged("PopViewModel"); }
        }

        private SelectionViewModel _foundViewModel;

        public SelectionViewModel FoundViewModel
        {
            get { return _foundViewModel; }
            private set { _foundViewModel = value; OnPropertyChanged("PopViewModel"); }
        }



        private SelectionViewModel _newViewModel;

        public SelectionViewModel NewViewModel
        {
            get { return _newViewModel; }
            private set { _newViewModel = value; OnPropertyChanged("NewViewModel"); }
        }


        private int _selectedIndex;
        public int SelectedIndex
        {
            get { return _selectedIndex; }
            set { _selectedIndex = value; OnPropertyChanged("SelectedIndex"); }
        }

    }
}
