using CosImg.ExHentai.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CosImg.ExHentai.ViewModel
{
    public class DownLoadViewModel
    {




        public List<DownLoadModel> DownLoadList
        {
            set
            {
                App.DownLoadList = value;
            }
            get
            {
                return App.DownLoadList;
            }
        }
    }
}
