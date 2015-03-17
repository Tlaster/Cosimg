using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TBase;
using Windows.UI.Xaml.Media.Imaging;

namespace CosImg.ExHentai.ViewModel
{
    public class ReadingViewModel : NotifyPropertyChanged
    {
        private string _headerEn;
        private string _link;
        private bool _isDownLoaded;
        private string _imagePage;
        private List<ExHentaiLib.Prop.ImageListInfo> _pageList;

        public ReadingViewModel(string link, string headerEn, bool isDownLoaded = false)
        {
            this._headerEn = headerEn;
            this._link = link;
            this._isDownLoaded = isDownLoaded;
            OnLoaded();
        }

        public ReadingViewModel(string link, string headerEn, string imagePage, bool isDownLoaded = false)
            : this(link, headerEn, isDownLoaded)
        {
            this._imagePage = imagePage;
        }

        private void OnLoaded()
        {
            
        }
    }
}
