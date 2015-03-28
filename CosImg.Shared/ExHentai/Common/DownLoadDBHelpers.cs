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
        public static async Task<bool> CheckDBFile()
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
        public static async Task<SQLiteAsyncConnection> GetDBConnection()
        {
            var conn = new SQLiteAsyncConnection(ApplicationData.Current.LocalFolder.Path + "\\Download.db");
            if (!await CheckDBFile())
            {
                await conn.CreateTableAsync<DownLoadInfo>();
            }
            return conn;
        }
        public static async void Add(DownLoadInfo item)
        {
            SQLiteAsyncConnection conn = await GetDBConnection();
            await conn.InsertAsync(item);
        }

        public static async void Delete(string hashStr)
        {
            SQLiteAsyncConnection conn = await GetDBConnection();
            var query = from item in conn.Table<DownLoadInfo>()
                        where item.HashString == hashStr
                        select item;
            await conn.DeleteAsync(await query.FirstOrDefaultAsync());
        }


        public static async void Modify(DownLoadInfo item)
        {
            SQLiteAsyncConnection conn = await GetDBConnection();
            await conn.UpdateAsync(item);
        }

        public static async Task<DownLoadInfo> QueryFromLink(string link)
        {
            SQLiteAsyncConnection conn = await GetDBConnection();
            var query = from item in conn.Table<DownLoadInfo>()
                        where item.PageUri == link
                        select item;
            return await query.FirstOrDefaultAsync();
        }


        public static async Task<bool> CheckItemisDownloaded(string hashStr)
        {
            SQLiteAsyncConnection conn = await GetDBConnection();
            var query = from item in conn.Table<DownLoadInfo>()
                        where item.HashString == hashStr && item.DownLoadComplete
                        select item;
            var result = await query.FirstOrDefaultAsync();
            return result != default(DownLoadInfo);
        }
        public static async Task<DownLoadInfo> Query(string hashStr)
        {
            SQLiteAsyncConnection conn = await GetDBConnection();
            var query = from item in conn.Table<DownLoadInfo>()
                        where item.HashString == hashStr
                        select item;
            return await query.FirstOrDefaultAsync();
        }

        public static async Task<List<DownLoadInfo>> Query()
        {
            SQLiteAsyncConnection conn = await GetDBConnection();
            var query = conn.Table<DownLoadInfo>();
            List<DownLoadInfo> result = await query.ToListAsync();
            return result;
        }
        public static async Task<List<DownLoadInfo>> GetList(bool isComplete = false)
        {
            SQLiteAsyncConnection conn = await GetDBConnection();
            var query = from a in conn.Table<DownLoadInfo>()
                        where a.DownLoadComplete == isComplete
                        select a;
            return await query.ToListAsync();
        }

    }
}
