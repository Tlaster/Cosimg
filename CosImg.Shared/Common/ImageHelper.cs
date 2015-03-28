using System;
using System.IO;
using System.Threading.Tasks;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Media.Imaging;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Storage;
using TBase.RT;
using TBase;

namespace CosImg.Common
{
    public static class ImageHelper
    {

        public async static Task<ulong> GetFolderSize(Windows.Storage.StorageFolder folder)
        {
            ulong size = 0;
            foreach (Windows.Storage.StorageFile thisFile in await folder.GetFilesAsync())
            {
                Windows.Storage.FileProperties.BasicProperties props = await thisFile.GetBasicPropertiesAsync();

                size += props.Size;
            }
            foreach (Windows.Storage.StorageFolder thisFolder in await folder.GetFoldersAsync())
            {
                size += await GetFolderSize(thisFolder);
            }

            return size;
        }

        public static async Task<ulong> GetCacheSize()
        {
            var folder = await ApplicationData.Current.TemporaryFolder.CreateFolderAsync("cache", CreationCollisionOption.OpenIfExists);
            return await GetFolderSize(folder);

            //if (folder.IsCommonFileQuerySupported(CommonFileQuery.OrderByName))
            //{
            //    StorageFileQueryResult folders = folder.CreateFileQuery(CommonFileQuery.OrderByName);
            //    var fileSizeTasks = (await folders.GetFilesAsync()).Select(async file => (await file.GetBasicPropertiesAsync()).Size);
            //    var sizes = await Task.WhenAll(fileSizeTasks);
            //    return sizes.Sum(l => (long)l);
            //}
            //else
            //{
            //    var a = (await folder.GetBasicPropertiesAsync()).Size;
            //    return (long)(await folder.GetBasicPropertiesAsync()).Size;
            //}
        }

        public static async Task ClearCache()
        {
            var folder = await ApplicationData.Current.TemporaryFolder.CreateFolderAsync("cache", CreationCollisionOption.OpenIfExists);
            await folder.DeleteAsync(StorageDeleteOption.PermanentDelete);
        }


        public static async Task<bool> CheckDownLoadFolder(string folderName)
        {
            try
            {
#if WINDOWS_PHONE_APP
                await (await ApplicationData.Current.LocalCacheFolder.CreateFolderAsync("download", CreationCollisionOption.OpenIfExists)).GetFolderAsync(folderName);

#else
                await (await ApplicationData.Current.LocalFolder.CreateFolderAsync("download", CreationCollisionOption.OpenIfExists)).GetFolderAsync(folderName);
#endif
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static async Task<bool> CheckCacheImage(string folderName, string fileName)
        {
            try
            {

                var folder = await (await ApplicationData.Current.TemporaryFolder.CreateFolderAsync("cache", CreationCollisionOption.OpenIfExists)).GetFolderAsync(folderName);
                await folder.GetFileAsync(fileName);
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

        public static async Task DeleDownloadFile(string folderName)
        {
            if (await CheckDownLoadFolder(folderName))
            {
#if WINDOWS_PHONE_APP
                var folder = await (await ApplicationData.Current.LocalCacheFolder.CreateFolderAsync("download", CreationCollisionOption.OpenIfExists)).GetFolderAsync(folderName);
#else       
            var folder = await (await ApplicationData.Current.LocalFolder.CreateFolderAsync("download", CreationCollisionOption.OpenIfExists)).GetFolderAsync(folderName);
#endif
                await folder.DeleteAsync(StorageDeleteOption.PermanentDelete);
            }
        }

        public static async Task<byte[]> GetDownLoadedImage(string folderName, string fileName)
        {
#if WINDOWS_PHONE_APP
            var folder = await (await ApplicationData.Current.LocalCacheFolder.CreateFolderAsync("download", CreationCollisionOption.OpenIfExists)).GetFolderAsync(folderName);
#else       
            var folder = await (await ApplicationData.Current.LocalFolder.CreateFolderAsync("download", CreationCollisionOption.OpenIfExists)).GetFolderAsync(folderName);
#endif
            var file = await folder.GetFileAsync(fileName);
            var imagebyte = default(byte[]);
            using (var stream = await file.OpenStreamForReadAsync())
            {
                imagebyte = await Converter.StreamToBytes(stream);
            }
            return imagebyte;
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


        public static async Task SaveDownLoadedImage(string folderName, string fileName, byte[] imagebyte)
        {
#if WINDOWS_PHONE_APP
            var folder = await (await ApplicationData.Current.LocalCacheFolder.CreateFolderAsync("download", CreationCollisionOption.OpenIfExists)).GetFolderAsync(folderName);
#else
            var folder = await (await ApplicationData.Current.LocalFolder.CreateFolderAsync("download", CreationCollisionOption.OpenIfExists)).GetFolderAsync(folderName);
#endif
            var file = await folder.CreateFileAsync(fileName, CreationCollisionOption.ReplaceExisting);
            await Windows.Storage.FileIO.WriteBytesAsync(file, imagebyte);
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
