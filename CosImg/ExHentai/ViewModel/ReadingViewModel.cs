using CosImg.ExHentai.Model;
using ExHentaiLib.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.Security.Cryptography;
using Windows.Security.Cryptography.Core;
using TBase;
using TBase.RT;

namespace CosImg.ExHentai.ViewModel
{
    public class ReadingViewModel:TBase.NotifyPropertyChanged
    {
        private string Link;
        private List<ExHentaiLib.Prop.ImageListInfo> PageList;
        private string HeaderEn;
        public ReadingViewModel(string link,string headerEn)
        {
            this.HeaderEn = headerEn;
            this.Link = link;
            ImageList = new List<ImageModel>();
            OnLoaded();
        }

        private async void OnLoaded()
        {
            this.PageList = await ParseHelper.GetImagePageListAsync(Link, SettingHelpers.GetSetting<string>("cookie"));
            var temp = new List<ImageModel>();
            for (int i = 0; i < PageList.Count; i++)
            {
                temp.Add(new ImageModel() 
                { 
                    ImageIndex = i,
                    ImagePage = PageList[i].ImagePage,
                    SaveFolder = HashedStringOf(HeaderEn) 
                });
            }
            ImageList = temp;
            OnPropertyChanged("ImageList");
        }

        private string HashedStringOf(string name)
        {
            Windows.Storage.Streams.IBuffer buff_utf8 = CryptographicBuffer.ConvertStringToBinary(name, Windows.Security.Cryptography.BinaryStringEncoding.Utf8);
            HashAlgorithmProvider algprov = HashAlgorithmProvider.OpenAlgorithm(HashAlgorithmNames.Sha1);
            Windows.Storage.Streams.IBuffer buff_hash = algprov.HashData(buff_utf8);
            return CryptographicBuffer.EncodeToHexString(buff_hash);
        }

        public int SelectIndex { get; set; }
        public ICommand RefreshCommand
        {
            get
            {
                return new DelegateCommand(() =>
                {
                    ImageList[SelectIndex].Refresh();
                });
            }
        }
        public ICommand SaveCommand
        {
            get
            {
                return new DelegateCommand(() =>
                {
                    ImageList[SelectIndex].Save();
                });
            }
        }
        public ICommand ShareCommand
        {
            get
            {
                return new DelegateCommand(() =>
                {
                    ImageList[SelectIndex].Share();
                });
            }
        }

        public List<ImageModel> ImageList { get; set; }
    }
}
