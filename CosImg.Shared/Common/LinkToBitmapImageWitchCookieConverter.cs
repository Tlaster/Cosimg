using ExHentaiLib.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using TBase;
using TBase.RT;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media.Imaging;

namespace CosImg.Common
{
    public class LinkToBitmapImageWitchCookieConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return new TaskCompletionNotifier<BitmapImage>(GetImage(value.ToString()));

        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
        private async Task<BitmapImage> GetImage(string link)
        {
            try
            {
                return await ImageHelper.ByteArrayToBitmapImage(await HttpHelper.GetByteArrayWithPostMethod(link, SettingHelpers.GetSetting<string>("cookie") + ParseHelper.unconfig));

            }
            catch (Exception)
            {
                return new BitmapImage();
            }

        }
    }
}
