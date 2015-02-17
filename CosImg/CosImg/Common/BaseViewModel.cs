using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace CosImg.CosImg.Common
{
    public abstract class BaseViewModel<T> : LoadProps
    {

        private GeneratorIncrementalLoadingClass<T> _list;

        public GeneratorIncrementalLoadingClass<T> List
        {
            get { return _list; }
            set { _list = value; OnPropertyChanged("List"); }
        }

        public string Link { get; set; }
        public abstract ICommand ReTryCommand { get; }
        public abstract ICommand ItemTapped { get; }

    }
}
