using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using TBase;
using Windows.UI.Xaml;

namespace CosImg.ExHentai.Model
{
    public class SearchModel : PivotItemModel
    {
        bool isEmpty = true;
        public override Windows.UI.Xaml.Visibility LoadMoreVis
        {
            get
            {
                if (isEmpty)
                {
                    return Visibility.Collapsed;
                }
                return !isOnLoading && !isLoadFail ? Visibility.Visible : Visibility.Collapsed;
            }
        }
        public ICommand SearchCommand
        {
            get
            {
                return new DelegateCommand<string>((str) =>
                {
                    List = new System.Collections.ObjectModel.ObservableCollection<ExHentaiLib.Prop.MainListProp>();
                    isEmpty = false;
                    _link = "http://exhentai.org/?f_doujinshi=1&f_manga=1&f_artistcg=1&f_gamecg=1&f_western=1&f_non-h=1&f_imageset=1&f_cosplay=1&f_asianporn=1&f_misc=1&f_search=" + str + "&f_apply=Apply+Filter";
                    OnLoaded(_link);
                });
            }
        }
    }
}
