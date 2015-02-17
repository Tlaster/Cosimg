using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExHentaiLib.Prop;

namespace ExHentaiLib.Interface
{
    public interface IDownLoadProp2T<T>
    {
        Task<T> ImageListInfo2T(ImageListInfo doanloadprop);
    }
}
