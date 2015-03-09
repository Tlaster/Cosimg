using CosImg.Common;
using ExHentaiLib.Prop;
using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.UI.Xaml.Media.Imaging;

namespace CosImg.ExHentai.Model
{
    [Table("DownLoadInfo")]
    public class DownLoadInfo : TBase.NotifyPropertyChanged
    {
        [AutoIncrement, PrimaryKey]
        public int PK { get; set; }
        public string HashString { get; set; }
        //public string HashString 
        //{
        //    get
        //    {
        //        return Name.GetHashedString();
        //    }
        //}
        public int MaxImageCount { get; protected set; }
        public int CurrentPage { get; set; }
        public string Name { get; set; }
        public string PageUri { get; set; }


        public byte[] Imagebyte { get; set; }

        private BitmapImage _image;
        public BitmapImage ItemImage
        {
            get
            {
                if (_image == null)
                {
                    LoadImage();
                    return null;
                }
                else
                {
                    return _image;
                }
            }
        }

        private async void LoadImage()
        {
            if (Imagebyte==null)
            {
                _image = new BitmapImage();
            }
            else
            {
                _image = await ImageHelper.ByteArrayToBitmapImage(Imagebyte);
            }
        }

    }
}
