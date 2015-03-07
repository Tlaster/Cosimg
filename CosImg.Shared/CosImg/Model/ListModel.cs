using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TBase.RT;
using Windows.Graphics.Display;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace CosImg.CosImg.Model
{
    public class ListModel
    {
        
        public double HeightWidth
        {
            get
            {
#if WINDOWS_PHONE_APP
                var a = SettingHelpers.GetSetting<int?>("ColumnCount") == default(int?) ? 3 : SettingHelpers.GetSetting<int>("ColumnCount");
                return ((Window.Current.Content as Frame).ActualWidth) / a - 10d;
#else
                return ((Window.Current.Content as Frame).ActualWidth) / 10;
#endif
            }
        }
        public string image { get; set; }

        public string name { get; set; }

        public string url { get; set; }

        public double id { get; set; }
    }
}
