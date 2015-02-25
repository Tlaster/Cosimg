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
using System.Diagnostics;

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


        public static async Task ClearCache()
        {
            var folder = await ApplicationData.Current.LocalCacheFolder.CreateFolderAsync("cache", CreationCollisionOption.OpenIfExists);
            await folder.DeleteAsync(StorageDeleteOption.PermanentDelete);
            Debug.WriteLine("cache deleted");
        }

        public static async Task<bool> CheckCacheImage(string folderName, string fileName)
        {
            try
            {
                var folder = await (await ApplicationData.Current.LocalCacheFolder.CreateFolderAsync("cache", CreationCollisionOption.OpenIfExists)).GetFolderAsync(folderName);
                var file = await folder.GetFileAsync(fileName);
                Debug.WriteLine("have cache");
                return true;
            }
            catch (Exception)
            {
                Debug.WriteLine("no cache");
                return false;
            }
        }



        public static async Task<byte[]> GetCacheImage(string folderName, string fileName)
        {
            var folder = await (await ApplicationData.Current.LocalCacheFolder.CreateFolderAsync("cache", CreationCollisionOption.OpenIfExists)).GetFolderAsync(folderName);
            var file = await folder.GetFileAsync(fileName);
            var imagebyte = default(byte[]);
            using (var stream = await file.OpenStreamForReadAsync())
            {
                imagebyte = await StreamToBytes(stream);
            }
            Debug.WriteLine("get cache");
            return imagebyte;
        }


        public static async Task SaveCacheImage(string folderName, string fileName, byte[] imagebyte)
        {
            var cachefolder = await ApplicationData.Current.LocalCacheFolder.CreateFolderAsync("cache",CreationCollisionOption.OpenIfExists);
            var folder = await cachefolder.CreateFolderAsync(folderName, CreationCollisionOption.OpenIfExists);
            var file = await folder.CreateFileAsync(fileName, CreationCollisionOption.ReplaceExisting);
            await Windows.Storage.FileIO.WriteBytesAsync(file, imagebyte);
            Debug.WriteLine("cached");
        }


        public static async Task SaveImage(string filename,byte[] imagebyte)
        {
            try
            {
                StorageFile destinationFile = await KnownFolders.SavedPictures.CreateFileAsync(filename, CreationCollisionOption.GenerateUniqueName);
                await Windows.Storage.FileIO.WriteBytesAsync(destinationFile, imagebyte);
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
                await bitmapImage.SetSourceAsync(stream);
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
        public static async Task<byte[]> GetImageByteArrayFromUriAsync(string imgUri,string cookie)
        {
            byte[] bit = default(byte[]);
            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("Cookie", cookie);
                client.DefaultRequestHeaders.Accept.TryParseAdd("text/html, application/xhtml+xml, */*");
                client.DefaultRequestHeaders.AcceptEncoding.TryParseAdd("gzip, deflate");
                client.DefaultRequestHeaders.AcceptLanguage.TryParseAdd("en-US,en;q=0.8,zh-Hans-CN;q=0.6,zh-Hans;q=0.4,ja;q=0.2");
                client.DefaultRequestHeaders.UserAgent.TryParseAdd("Mozilla/5.0 (MSIE 9.0; Windows NT 6.3; WOW64; Trident/7.0; rv:11.0) like Gecko");
                bit = await client.GetByteArrayAsync(imgUri);
            }
            return bit;
        }


    }
}
