using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TBase;
using TBase.RT;
using Windows.Data.Json;

namespace CosImg.CosImg.Common
{
    public class GeneratorIncrementalLoadingClass<T> : IncrementalLoadingBase<T>
    {

        public GeneratorIncrementalLoadingClass(string uri, Func<IJsonValue, T> generator, int loadCount = 20)
        {
            _uri = uri;
            _loadCount = loadCount;
            _generator = generator;
        }


        protected async override Task<IList<T>> LoadMoreItemsOverrideAsync(System.Threading.CancellationToken c, uint count)
        {
            var random = new Random();
            var str = await HttpHelper.HttpReadString(_uri + "&page=" + _nextPage + "&limit=" + _loadCount.ToString() + "&p3_photo_list=1&r=" + random.Next());
            JsonObject jsonObj = JsonObject.Parse(str);
            var itemResult = from a in jsonObj["list"].GetArray()
                             select _generator(a);
            _currrentPage = jsonObj["pager"].GetObject()["current_page"].Stringify().Replace("\"", "");
            _nextPage = jsonObj["pager"].GetObject()["next_page"].ValueType == JsonValueType.Null ?
                _nextPage = null : jsonObj["pager"].GetObject()["next_page"].Stringify().Replace("\"", "");
            return itemResult.ToArray();
        }

        protected override bool HasMoreItemsOverride()
        {
            return _nextPage != null;
        }

        #region State

        Func<IJsonValue, T> _generator;
        string _currrentPage = "0";
        string _nextPage = "1";
        private string _uri;
        private int _loadCount;

        #endregion


    }

}
