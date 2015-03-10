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
    public static class FavorDBHelpers
    {
        public static async Task<bool> CheckDBFile()
        {
            try
            {
                await ApplicationData.Current.LocalFolder.GetFileAsync("Favor.db");
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        public static async Task<SQLiteAsyncConnection> GetDBConnection()
        {
            var conn = new SQLiteAsyncConnection(ApplicationData.Current.LocalFolder.Path + "\\Favor.db");
            if (!await CheckDBFile())
            {
                await conn.CreateTableAsync<FavorModel>();
            }
            return conn;
        }
        public static async void Add(FavorModel item)
        {
            SQLiteAsyncConnection conn = await GetDBConnection();
            await conn.InsertAsync(item);
        }

        public static async void Delete(string hashStr)
        {
            SQLiteAsyncConnection conn = await GetDBConnection();
            var query = from item in conn.Table<FavorModel>()
                        where item.HashString == hashStr
                        select item;
            await conn.DeleteAsync(await query.FirstOrDefaultAsync());
        }
        public static async void Modify(FavorModel item)
        {
            SQLiteAsyncConnection conn = await GetDBConnection();
            await conn.UpdateAsync(item);
        }

        public static async Task<FavorModel> Query(string hashStr)
        {
            SQLiteAsyncConnection conn = await GetDBConnection();
            var query = from item in conn.Table<FavorModel>()
                        where item.HashString == hashStr
                        select item;
            return await query.FirstOrDefaultAsync();
        }

        public static async Task<List<FavorModel>> Query()
        {
            SQLiteAsyncConnection conn = await GetDBConnection();
            var query = conn.Table<FavorModel>();
            List<FavorModel> result = await query.ToListAsync();
            return result;

        }


    }
}
