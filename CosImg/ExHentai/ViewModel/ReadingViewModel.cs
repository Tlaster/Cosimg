using CosImg.ExHentai.Model;
using ExHentaiLib.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using TBase;
using TBase.RT;

namespace CosImg.ExHentai.ViewModel
{
    public class ReadingViewModel:TBase.NotifyPropertyChanged
    {
        private string Link;
        private List<ExHentaiLib.Prop.ImageListInfo> PageList;
        public ReadingViewModel(string link)
        {
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
                    ImagePage = PageList[i].ImagePage,
                    //Image = await ParseHelper.GetImageAync(PageList[i].ImagePage, TBase.RT.SettingHelpers.GetSetting<string>("cookie")) 
                });
            }
            ImageList = temp;
            OnPropertyChanged("ImageList");
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
