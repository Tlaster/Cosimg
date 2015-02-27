using ExHentaiLib.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Markup;
using System.Windows.Media.Imaging;
using TBase;

namespace ExHentaiViewer.WPF.Common
{
    public class LinkToBitmapImageWitchCookieConverter :MarkupExtension, IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return new TaskCompletionNotifier<BitmapImage>(GetImage((string)value));
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
        private async Task<BitmapImage> GetImage(string link)
        {
            using (WebClient client = new WebClient())
            {
                client.Headers["Cookie"] = CookieHelper.GetCookie();
                byte[] bit = await client.DownloadDataTaskAsync(link);
                return ByteArrayToBitmapImage(bit);
            }

        }

        private  BitmapImage ByteArrayToBitmapImage(byte[] byteArray)
        {
            var bitmapImage = new BitmapImage();
            bitmapImage.BeginInit();
            bitmapImage.StreamSource = new MemoryStream(byteArray);
            bitmapImage.EndInit();
            return bitmapImage;
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return this;
        }
    }
}
