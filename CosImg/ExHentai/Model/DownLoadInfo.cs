using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace CosImg.ExHentai.Model
{
    [Table("DownLoadInfo")]
    public class DownLoadInfo : TBase.NotifyPropertyChanged
    {
        [AutoIncrement, PrimaryKey]
        public int PK { get; set; }
        public string HashString { get; protected set; }
        public StorageFolder _saveFolder { get; protected set; }
        public int MaxImageCount { get; protected set; }
        public int CurrentPage { get; protected set; }
        public string Name { get; protected set; }
    }
}
