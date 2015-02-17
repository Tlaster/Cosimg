using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExHentaiLib.Interface;
using ExHentaiLib.Prop;
using TBase;

namespace ExHentaiLib.Common
{
    public abstract class ReadingModel<T> : NotifyPropertyChanged, IDownLoadProp2T<T>
    {
        public List<ImageListInfo> imageList;
        public List<T> ViewList { get; set; }

        private int _currentPageIndex = 1;
        public int CurrentPageIndex
        {
            get { return _currentPageIndex; }
            set { _currentPageIndex = value; OnPropertyChanged("CurrentPageIndex"); }
        }


        private int _maxImageCount;
        public int MaxImageCount
        {
            get { return _maxImageCount; }
            set
            {
                _maxImageCount = value;
                OnPropertyChanged("MaxImageCount");
            }
        }
        public async Task LoadImageList(int selectPage)
        {
            CurrentPageIndex = selectPage;
            var viewListCount = imageList.Count < 5 ? imageList.Count : 5;
            var minPage = selectPage - 2 < 0 ? 0 : selectPage - 2;
            var maxPage = selectPage + 2 > imageList.Count ? imageList.Count : selectPage + 2;
            for (int i = minPage; i < maxPage; i++)
            {
                ViewList.Add(await ImageListInfo2T(imageList[i]));
            }
        }

        public async Task LoadImageList()
        {
            var viewListCount = imageList.Count <= 2 ? imageList.Count : 2;
            for (int i = 0; i < viewListCount; i++)
            {
                ViewList.Add(await ImageListInfo2T(imageList[i]));
            }
        }
        /// <summary>
        /// TODO:Override it ,Convert DownLoadProp to T like below
        /// <para>{</para>
        /// <para>    return new ViewImageProp() { ImageName = doanloadprop.FlieName, ImageUri = await ParseHelper.GetImageAync(doanloadprop.ImagePage, GetCookie()) };</para>
        /// <para>}</para>
        /// </summary>
        /// <param name="doanloadprop"></param>
        /// <returns></returns>
        public abstract Task<T> ImageListInfo2T(ImageListInfo doanloadprop);

        public async Task MoveNext()
        {
            if (CurrentPageIndex==imageList.Count-1)
            {
                return;
            }
            CurrentPageIndex++;
            if (CurrentPageIndex >= imageList.Count - 2)
            {
                return;
            }
            AddLast(await ImageListInfo2T(imageList[CurrentPageIndex]));
        }

        public async Task MoveForward()
        {
            if (CurrentPageIndex==0)
            {
                return;
            }
            CurrentPageIndex--;
            if (CurrentPageIndex < 1)
            {
                return;
            }
            AddFirst(await ImageListInfo2T(imageList[CurrentPageIndex-1]));
        }

        private void AddLast(T item)
        {
            ViewList.RemoveAt(0);
            ViewList.Add(item);
        }

        private void AddFirst(T item)
        {
            ViewList.RemoveAt(ViewList.Count - 1);
            ViewList.Insert(0, item);
        }

    }
}
