 using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExHentaiLib.Prop
{
    public class DetailProp:TBase.NotifyPropertyChanged
    {
        public HeaderInfo HeaderInfo { get; set; }

        public int DetailPageCount { get; set; }

        public List<string> PageCountList
        {
            get
            {
                List<string> _pageCountlist = new List<string>();
                for (int i = 1; i <= DetailPageCount; i++)
                {
                    _pageCountlist.Add(i.ToString());
                }
                return _pageCountlist;
            }
        }

        private List<ImageListInfo> _imageList;

        public List<ImageListInfo> ImageList
        {
            get { return _imageList; }
            set 
            {
                _imageList = value; 
                OnPropertyChanged("ImageList"); 
            }
        }


        public string ImageCountString { get; set; }
    }
}
