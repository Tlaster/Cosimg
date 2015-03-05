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
using TBase;

namespace CosImg.Common
{
    public static class ImageHelper
    {
        public static async Task ShareImage(byte[] imagebyte, string content = "分享图片")
        {
            UmengPicture picture = new UmengPicture(imagebyte, content);
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
            var folder = await ApplicationData.Current.TemporaryFolder.CreateFolderAsync("cache", CreationCollisionOption.OpenIfExists);
            await folder.DeleteAsync(StorageDeleteOption.PermanentDelete);
        }

        public static async Task<bool> CheckCacheImage(string folderName, string fileName)
        {
            try
            {
                var folder = await (await ApplicationData.Current.TemporaryFolder.CreateFolderAsync("cache", CreationCollisionOption.OpenIfExists)).GetFolderAsync(folderName);
                var file = await folder.GetFileAsync(fileName);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }


        public static async Task DeleCacheImage(string folderName, string fileName)
        {
            if (await CheckCacheImage(folderName,fileName))
            {
                var folder = await (await ApplicationData.Current.TemporaryFolder.CreateFolderAsync("cache", CreationCollisionOption.OpenIfExists)).GetFolderAsync(folderName);
                var file = await folder.GetFileAsync(fileName);
                await file.DeleteAsync(StorageDeleteOption.PermanentDelete);
            }
        }


        public static async Task<byte[]> GetCacheImage(string folderName, string fileName)
        {
            var folder = await (await ApplicationData.Current.TemporaryFolder.CreateFolderAsync("cache", CreationCollisionOption.OpenIfExists)).GetFolderAsync(folderName);
            var file = await folder.GetFileAsync(fileName);
            var imagebyte = default(byte[]);
            using (var stream = await file.OpenStreamForReadAsync())
            {
                imagebyte = await Converter.StreamToBytes(stream);
            }
            return imagebyte;
        }


        public static async Task SaveCacheImage(string folderName, string fileName, byte[] imagebyte)
        {
            var cachefolder = await ApplicationData.Current.TemporaryFolder.CreateFolderAsync("cache", CreationCollisionOption.OpenIfExists);
            var folder = await cachefolder.CreateFolderAsync(folderName, CreationCollisionOption.OpenIfExists);
            var file = await folder.CreateFileAsync(fileName, CreationCollisionOption.ReplaceExisting);
            await Windows.Storage.FileIO.WriteBytesAsync(file, imagebyte);
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





    }
}
