using CosImg.CosImg.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using TBase.RT;
using TBase;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml.Media.Imaging;
using Windows.Storage;
using Windows.Storage.Streams;
using System.Threading.Tasks;
using System.Net;
using Windows.Web.Http;
using UmengSocialSDK;

// “空白页”项模板在 http://go.microsoft.com/fwlink/?LinkID=390556 上有介绍

namespace CosImg.CosImg.View
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class ImagePage : Page
    {
        private ListModel Item;
        private byte[] ImageByte;
        public ImagePage()
        {
            this.InitializeComponent();
            this.imageGrid.Width = (Window.Current.Content as Frame).ActualWidth;
            this.imageGrid.Height = (Window.Current.Content as Frame).ActualHeight;
            this.BigImage.Width = (Window.Current.Content as Frame).ActualWidth;
        }

        /// <summary>
        /// 在此页将要在 Frame 中显示时进行调用。
        /// </summary>
        /// <param name="e">描述如何访问此页的事件数据。
        /// 此参数通常用于配置页。</param>
        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            proRing.IsActive = true;
            Item = e.Parameter as ListModel;
            var htmlStr = await HttpHelper.HttpReadString("http://worldcosplay.net" + Item.url);
            HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
            doc.LoadHtml(htmlStr);
            if (Item.url.Contains("instants"))
            {
                var node = doc.DocumentNode.GetNodebyClassName("photo");
                var a = node.Attributes["src"].Value;
                BitmapImage img = new BitmapImage(new Uri(node.Attributes["src"].Value));
                //this.BigImage.Source = img;
                ImageByte = await GetImageByteArrayFromUriAsync(node.Attributes["src"].Value);
            }
            else
            {
                var node = doc.DocumentNode.GetNodebyClassName("img");
                var a = node.Attributes["src"].Value;
                BitmapImage img = new BitmapImage(new Uri(node.Attributes["src"].Value));
                //this.BigImage.Source = img;
                ImageByte = await GetImageByteArrayFromUriAsync(node.Attributes["src"].Value);
            }
            proRing.IsActive = false;
        }

        private static async Task<BitmapImage> ByteArrayToBitmapImage(byte[] byteArray)
        {
            var bitmapImage = new BitmapImage();
            using (var stream = new InMemoryRandomAccessStream())
            {
                await stream.WriteAsync(byteArray.AsBuffer());
                stream.Seek(0);
                bitmapImage.SetSource(stream);
            }
            return bitmapImage;
        }

        private async Task<byte[]> GetImageByteArrayFromUriAsync(string imgUri)
        {
            byte[] bit = default(byte[]);
            using (HttpClient client = new HttpClient())
            {
                var stream = await client.GetInputStreamAsync(new Uri(imgUri));
                using (Stream resStream = stream.AsStreamForRead())
                {
                    bit = await StreamToBytes(resStream);
                    this.BigImage.Source = await ByteArrayToBitmapImage(bit);
                }
            }
            return bit;
        }

        private async Task<byte[]> StreamToBytes(Stream input)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                await input.CopyToAsync(ms);
                return ms.ToArray();
            }
        }

        private async void AppBarButton_Click_1(object sender, RoutedEventArgs e)
        {
            if (this.BigImage.Source == null)
            {
                new ToastPrompt("Please Wait While The Image Is Loading").Show();
                return;
            }

            UmengPicture picture = new UmengPicture(ImageByte, "分享图片");

            List<UmengClient> clients = new List<UmengClient>() 
                    { 
                        new SinaWeiboClient("549eb88bfd98c51269000c35"), 
                        new RenrenClient("549eb88bfd98c51269000c35"), 
                        new QzoneClient("549eb88bfd98c51269000c35"),
                        new TencentWeiboClient("549eb88bfd98c51269000c35"), 
                        new DoubanClient("549eb88bfd98c51269000c35"),
                    };
            UmengClient umengClient = new MultiClient(clients);
            var res = await umengClient.SharePictureAsync(picture);
            switch (res.Status)
            {
                case ShareStatus.UserCanceled:
                case ShareStatus.Unsupported:
                case ShareStatus.Error:
                    new ToastPrompt("分享失败").Show();
                    break;
                case ShareStatus.OAuthExpired:
                    new ToastPrompt("分享失败，授权失效").Show();
                    break;
                case ShareStatus.Success:
                    new ToastPrompt("分享成功").Show();
                    break;
                default:
                    break;
            }
        }

        private async void AppBarButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                BitmapImage bi = this.BigImage.Source as BitmapImage;
                var urisource = bi.UriSource;
                StorageFile destinationFile = await KnownFolders.SavedPictures.CreateFileAsync(Path.GetFileName(urisource.ToString()), CreationCollisionOption.GenerateUniqueName);
                var sourceFile = RandomAccessStreamReference.CreateFromUri(urisource);
                using (var sourceStream = await sourceFile.OpenReadAsync())
                {
                    using (var sourceInputStream = sourceStream.GetInputStreamAt(0))
                    {
                        using (var destinationStream = await destinationFile.OpenAsync(FileAccessMode.ReadWrite))
                        {
                            using (var destinationOutputStream = destinationStream.GetOutputStreamAt(0))
                            {
                                await RandomAccessStream.CopyAndCloseAsync(sourceInputStream, destinationStream);
                            }
                        }
                    }
                }
                new ToastPrompt("保存成功").Show();
            }
            catch (Exception)
            {
                new ToastPrompt("保存失败").Show();
            }
        }

    }
}
