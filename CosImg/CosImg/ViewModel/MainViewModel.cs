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

        private List<SelectionModel> _newList= new List<SelectionModel>()
        { 
            new SelectionModel() { Name = "最新照片", Link = "http://worldcosplay.net/api/photo/list?",Generator = generator }, 
            new SelectionModel() { Name = "最新随拍", Link = "http://worldcosplay.net/api/instants/list?",Generator = generator2 }
        };
        public List<SelectionModel> NewList
        {
            get { return _newList; }
            set { _newList = value; }
        }

        public ICommand NewListPicked
        {
            get
            {
                return new DelegateCommand<SelectionModel>((item) =>
                {
                    NewViewModel = new SelectionViewModel<ListModel>(item.Link, item.Generator);
                });
            }
        }

        private List<SelectionModel> _popList = new List<SelectionModel>()
        {
            new SelectionModel(){ Name="按日统计",Link="http://worldcosplay.net/api/ranking/good?sort=daily_good_cnt",Generator=generator},
            new SelectionModel(){ Name="按周统计",Link="http://worldcosplay.net/api/ranking/good?sort=weekly_good_cnt",Generator=generator},
            new SelectionModel(){ Name="按月统计",Link="http://worldcosplay.net/api/ranking/good?sort=monthly_good_cnt",Generator=generator},
            new SelectionModel(){ Name="累计",Link="http://worldcosplay.net/api/ranking/good?sort=good_cnt",Generator=generator},
        };

        public List<SelectionModel> PopList
        {
            get { return _popList; }
            set { _popList = value; }
        }
        public ICommand PopListPicked
        {
            get
            {
                return new DelegateCommand<SelectionModel>((item) =>
                {
                    PopViewModel = new SelectionViewModel<ListModel>(item.Link, item.Generator);
                });
            }
        }



        public MainViewModel()
        {
            FoundViewModel = new SelectionViewModel<ListModel>("http://worldcosplay.net/api/photo/popular?", generator);
            NewViewModel = new SelectionViewModel<ListModel>("http://worldcosplay.net/api/photo/list?", generator);
            PopViewModel = new SelectionViewModel<ListModel>("http://worldcosplay.net/api/ranking/good?sort=daily_good_cnt", generator);
            SearchViewModel = new SearchModel<ListModel>(generator);
        }

        public SearchModel<ListModel> SearchViewModel { get; set; }


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

        private SelectionViewModel<ListModel> _popViewModel;

        public SelectionViewModel<ListModel> PopViewModel
        {
            get { return _popViewModel; }
            private set { _popViewModel = value; OnPropertyChanged("PopViewModel"); }
        }

        private SelectionViewModel<ListModel> _foundViewModel;

        public SelectionViewModel<ListModel> FoundViewModel
        {
            get { return _foundViewModel; }
            private set { _foundViewModel = value; OnPropertyChanged("PopViewModel"); }
        }



        private SelectionViewModel<ListModel> _newViewModel;

        public SelectionViewModel<ListModel> NewViewModel
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
