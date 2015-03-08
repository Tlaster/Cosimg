using CosImg.ExHentai.Model;
using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace CosImg.ExHentai.Common
{
    public static class DownLoadDBHelpers
    {
        public static async Task<bool> CheckFavorDBFile()
        {
            try
            {
                await ApplicationData.Current.LocalFolder.GetFileAsync("Download.db");
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        public static async Task<SQLiteAsyncConnection> GetFavorDBConnection()
        {
            var conn = new SQLiteAsyncConnection(ApplicationData.Current.LocalFolder.Path + "\\Download.db");
            if (!await CheckFavorDBFile())
            {
                await conn.CreateTableAsync<DownLoadInfo>();
            }
            return conn;
        }
        public static async void Add(DownLoadInfo item)
        {
            SQLiteAsyncConnection conn = await GetFavorDBConnection();
            await conn.InsertAsync(item);
        }

        public static async void Delete(string hashStr)
        {
            SQLiteAsyncConnection conn = await GetFavorDBConnection();
            var query = from item in conn.Table<DownLoadInfo>()
                        where item.HashString == hashStr
                        select item;
            await conn.DeleteAsync(await query.FirstOrDefaultAsync());
        }

        public static async void Modify(string hashStr)
        {
            SQLiteAsyncConnection conn = await GetFavorDBConnection();
            await conn.UpdateAsync(Query(hashStr));
        }

        public static async void Modify(DownLoadInfo item)
        {
            SQLiteAsyncConnection conn = await GetFavorDBConnection();
            await conn.UpdateAsync(item);
        }

        public static async Task<DownLoadInfo> Query(string hashStr)
        {
            SQLiteAsyncConnection conn = await GetFavorDBConnection();
            var query = from item in conn.Table<DownLoadInfo>()
                        where item.HashString == hashStr
                        select item;
            return await query.FirstOrDefaultAsync();
        }

        public static async Task<List<DownLoadInfo>> Query()
        {
            SQLiteAsyncConnection conn = await GetFavorDBConnection();
            var query = conn.Table<DownLoadInfo>();
            List<DownLoadInfo> result = await query.ToListAsync();
            return result;

        }
    }
}
