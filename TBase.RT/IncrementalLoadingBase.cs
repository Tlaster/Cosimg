using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;

namespace TBase.RT
{
    public abstract class IncrementalLoadingBase<T> : IList, ISupportIncrementalLoading, INotifyCollectionChanged
    {
        #region IList

        public int Add(object value)
        {
            _storage.Add((T)value);
            return _storage.Count - 1;
        }

        public void Clear()
        {
            _storage.Clear();
        }

        public bool Contains(object value)
        {
            return _storage.Contains((T)value);
        }

        public int IndexOf(object value)
        {
            return _storage.IndexOf((T)value);
        }

        public void Insert(int index, object value)
        {
            _storage.Insert(index, (T)value);
        }

        public bool IsFixedSize
        {
            get { return false; }
        }

        public bool IsReadOnly
        {
            get { return true; }
        }

        public void Remove(object value)
        {
            _storage.Remove((T)value);
        }

        public void RemoveAt(int index)
        {
            _storage.RemoveAt(index);
        }

        public object this[int index]
        {
            get
            {
                return _storage[index];
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public void CopyTo(Array array, int index)
        {
            ((IList)_storage).CopyTo(array, index);
        }

        public int Count
        {
            get { return _storage.Count; }
        }

        public bool IsSynchronized
        {
            get { return false; }
        }

        public object SyncRoot
        {
            get { throw new NotImplementedException(); }
        }

        public IEnumerator GetEnumerator()
        {
            return _storage.GetEnumerator();
        }

        #endregion

        #region ISupportIncrementalLoading

        public bool HasMoreItems
        {
            get
            {
                if (_isFailed)
                {
                    return false;
                }
                else
                {
                    return HasMoreItemsOverride();
                }
            }
        }

        public Windows.Foundation.IAsyncOperation<LoadMoreItemsResult> LoadMoreItemsAsync(uint count)
        {
            if (_busy)
            {
                throw new InvalidOperationException("Only one operation in flight at a time");
            }

            _busy = true;

            return AsyncInfo.Run((c) => LoadMoreItemsAsync(c, count));
        }

        #endregion

        #region INotifyCollectionChanged

        public event NotifyCollectionChangedEventHandler CollectionChanged;

        #endregion

        #region Private methods

        async Task<LoadMoreItemsResult> LoadMoreItemsAsync(CancellationToken c, uint count)
        {
            try
            {
                if (OnLoading != null)
                {
                    OnLoading(this, null);
                }
                var items = await LoadMoreItemsOverrideAsync(c, count);
                var baseIndex = _storage.Count;

                _storage.AddRange(items);

                // Now notify of the new items
                NotifyOfInsertedItems(baseIndex, items.Count);
                if (LoadSucceed != null)
                {
                    LoadSucceed(this, null);
                }
                return new LoadMoreItemsResult { Count = (uint)items.Count };
            }
            catch (Exception e)
            {
                if (LoadFailed != null)
                {
                    LoadFailed(this, e);
                }
                _isFailed = true;
                return new LoadMoreItemsResult();
            }
            finally
            {
                _busy = false;
            }
        }

        void NotifyOfInsertedItems(int baseIndex, int count)
        {
            if (CollectionChanged == null)
            {
                return;
            }

            for (int i = 0; i < count; i++)
            {
                var args = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, _storage[i + baseIndex], i + baseIndex);
                CollectionChanged(this, args);
            }
        }

        #endregion

        #region Overridable methods

        protected abstract Task<IList<T>> LoadMoreItemsOverrideAsync(CancellationToken c, uint count);
        protected abstract bool HasMoreItemsOverride();

        #endregion

        #region State

        List<T> _storage = new List<T>();
        bool _busy = false;
        public event EventHandler<Exception> LoadFailed;
        public event EventHandler OnLoading;
        public event EventHandler LoadSucceed;
        bool _isFailed = false;

        #endregion

        //#region IList<T>
        //public int IndexOf(T item)
        //{
        //    return _storage.IndexOf(item);
        //}

        //public void Insert(int index, T item)
        //{
        //    _storage.Insert(index, item);
        //}

        //T IList<T>.this[int index]
        //{
        //    get
        //    {
        //        return _storage[index];
        //    }
        //    set
        //    {
        //        throw new NotImplementedException();
        //    }
        //}

        //public void Add(T item)
        //{
        //    _storage.Add(item);
        //}

        //public bool Contains(T item)
        //{
        //    return _storage.Contains(item);
        //}

        //public void CopyTo(T[] array, int arrayIndex)
        //{
        //    _storage.CopyTo(array, arrayIndex);
        //}

        //public bool Remove(T item)
        //{
        //    return _storage.Remove(item);
        //}

        //IEnumerator<T> IEnumerable<T>.GetEnumerator()
        //{
        //    return _storage.GetEnumerator();
        //}
        //#endregion
    }
}
