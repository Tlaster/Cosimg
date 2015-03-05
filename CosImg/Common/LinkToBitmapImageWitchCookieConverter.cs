using ExHentaiLib.Common;
using System;
using System.Collections.Generic;
using System.Linq;
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
            //return new TaskCompletionNotifier<BitmapImage>(Task.Run<BitmapImage>(async () =>
            //{
            //    return await ImageHelper.ByteArrayToBitmapImage(await HttpHelper.GetByteArray(value.ToString(), SettingHelpers.GetSetting<string>("cookie") + ParseHelper.unconfig));
            //}));
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
        private async Task<BitmapImage> GetImage(string link)
        {
            try
            {
                var bit = await HttpHelper.GetByteArray(link, SettingHelpers.GetSetting<string>("cookie") + ParseHelper.unconfig);
                return await ImageHelper.ByteArrayToBitmapImage(bit);
                //using (HttpClient client = new HttpClient())
                //{
                //    client.DefaultRequestHeaders.Add("Cookie", SettingHelpers.GetSetting<string>("cookie") + ParseHelper.unconfig);
                //    byte[] bit = await client.GetByteArrayAsync(link);
                //    return await ImageHelper.ByteArrayToBitmapImage(bit);
                //}
            }
            catch (Exception e)
            {
                return new BitmapImage();
            }

        }
    }
}
