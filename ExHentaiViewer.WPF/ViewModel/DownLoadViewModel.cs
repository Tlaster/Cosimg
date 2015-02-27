using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using ExHentaiLib.Common;
using ExHentaiLib.Prop;
using ExHentaiViewer.WPF.Prop;
using TBase;

namespace ExHentaiViewer.WPF.ViewModel
{
    public class DownLoadViewModel:NotifyPropertyChanged
    {
        public DownLoadViewModel()
        {
            DownLoadList = new ObservableCollection<DownLoadListProp>();
        }

        private ObservableCollection<DownLoadListProp> _downLoadList;
        public ObservableCollection<DownLoadListProp> DownLoadList
        {
            get { return _downLoadList; }
            set 
            {
                _downLoadList = value;
                OnPropertyChanged("DownLoadList");
            }
        }
        public ICommand ParseDownLoad
        {
            get
            {
                return new DelegateCommand((itemIndex) =>
                {
                    var index = (int)itemIndex; 
                    DownLoadList[index].CurrentState = "On Parsing...";
                    
                    DownLoadList[index].OnParse = DownLoadList[index].OnParse == true ? false : true;
                });
            }
        }
    

    }
}
