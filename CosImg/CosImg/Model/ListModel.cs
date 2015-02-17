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
                var a = SettingHelpers.GetSetting<int?>("ColumnCount") == default(int?) ? 3 : SettingHelpers.GetSetting<int>("ColumnCount");
                return ((Window.Current.Content as Frame).ActualWidth) / a - 10d;
                
                //var bounds = Window.Current.Bounds;
                //var dpiRatio = DisplayInformation.GetForCurrentView().RawPixelsPerViewPixel;
                //var resolutionH = Math.Round(bounds.Height * dpiRatio);
                //var resolutionW = Math.Round(bounds.Width * dpiRatio);
                //if (resolutionH == 1920d && resolutionW == 1080d)
                //{
                //    return ((Window.Current.Content as Frame).ActualWidth) / 4 - 10d;
                //}
                //else
                //{
                //}
            }
        }
        public string image { get; set; }

        public string name { get; set; }

        public string url { get; set; }

        public double id { get; set; }
    }
}
