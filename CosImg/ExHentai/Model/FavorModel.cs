using CosImg.Common;
using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TBase;
using TBase.RT;
using Windows.UI.Xaml.Media.Imaging;

namespace CosImg.ExHentai.Model
{
    [Table("FavorModel")]
    public class FavorModel:TBase.NotifyPropertyChanged
    {
        [AutoIncrement, PrimaryKey]
        public int PK { get; set; }
        public string HashString { get; set; }
        public string Name { get; set; }
        public string ItemPageLink { get; set; }
        public BitmapImage ItemImage
        {
            get
            {
                if (_itemImage==null)
                {
                    LoadImage();
                    return null;
                }
                else
                {
                    return _itemImage;
                }
            }
        }

        private async void LoadImage()
        {
            _itemImage = await ImageHelper.ByteArrayToBitmapImage(ImageByte);
            OnPropertyChanged("ItemImage");
        }
        public bool isDownLoaded { get; set; }
        private BitmapImage _itemImage;
        public byte[] ImageByte { get; set; }
    }
}
