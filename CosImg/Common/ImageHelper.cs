using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Media.Imaging;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Net.Http;
using System.Net;
using Windows.Storage;
using TBase.RT;
using UmengSocialSDK;

namespace CosImg.Common
{
    public static class ImageHelper
    {
        public static async Task ShareImage(byte[] imagebyte)
        {
            UmengPicture picture = new UmengPicture(imagebyte, "分享图片");
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

        public static async Task SaveImage(BitmapImage bitmapImage)
        {
            try
            {
                var urisource = bitmapImage.UriSource;
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
        public static async Task<BitmapImage> ByteArrayToBitmapImage(byte[] byteArray)
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
        public static async Task<byte[]> StreamToBytes(Stream input)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                await input.CopyToAsync(ms);
                return ms.ToArray();
            }
        }
        public static async Task<byte[]> GetImageByteArrayFromUriAsync(string imgUri)
        {
            byte[] bit = default(byte[]);
            using (HttpClient client = new HttpClient())
            {
                bit = await client.GetByteArrayAsync(imgUri);
            }
            return bit;
        }
    }
}
