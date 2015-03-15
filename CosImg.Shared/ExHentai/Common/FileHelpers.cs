using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace CosImg.ExHentai.Common
{
    public static class FileHelpers
    {



        public static async Task SaveDetailCache(string folderName, string content)
        {

#if WINDOWS_PHONE_APP
            var folder = await (await ApplicationData.Current.LocalCacheFolder.CreateFolderAsync("download", CreationCollisionOption.OpenIfExists)).CreateFolderAsync(folderName, CreationCollisionOption.OpenIfExists);

#else
            var folder = await (await ApplicationData.Current.LocalFolder.CreateFolderAsync("download", CreationCollisionOption.OpenIfExists)).CreateFolderAsync(folderName,CreationCollisionOption.OpenIfExists);
#endif
            var file = await folder.CreateFileAsync("detail", CreationCollisionOption.ReplaceExisting);
            await FileIO.WriteTextAsync(file, content);

        }
        public static async Task<string> GetDetailCache(string folderName)
        {
            try
            {

#if WINDOWS_PHONE_APP
                var folder = await (await ApplicationData.Current.LocalCacheFolder.CreateFolderAsync("download", CreationCollisionOption.OpenIfExists)).GetFolderAsync(folderName);

#else
                var folder = await (await ApplicationData.Current.LocalFolder.CreateFolderAsync("download", CreationCollisionOption.OpenIfExists)).GetFolderAsync(folderName);
#endif
                var file = await folder.GetFileAsync("detail");
                return await FileIO.ReadTextAsync(file);
                
            }
            catch (Exception)
            {
                return null;
            }
        }

    }
}
