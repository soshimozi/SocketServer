using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Specialized;

namespace ServiceHandlerLib.Collections
{
    public class AdaptingCollection<T, M> : ICollection<Lazy<T, M>>, INotifyCollectionChanged
    {
        private readonly List<Lazy<T, M>> _allItems = new List<Lazy<T, M>>();
        private readonly Func<IEnumerable<Lazy<T, M>>, IEnumerable<Lazy<T, M>>> _adaptor = null;
        private List<Lazy<T, M>> _adaptedItems = null;

        public AdaptingCollection()
            : this(null)
        {
        }

        public AdaptingCollection(Func<IEnumerable<Lazy<T, M>>, IEnumerable<Lazy<T, M>>> adaptor)
        {
            _adaptor = adaptor;
        }

        public event NotifyCollectionChangedEventHandler CollectionChanged;

        public void ReApplyAdaptor()
        {
            if( _adaptedItems != null )
            {
                _adaptedItems = null;
                OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
            }
        }

        protected virtual IEnumerable<Lazy<T, M>> Adapt(IEnumerable<Lazy<T, M>> collection)
        {
            if (_adaptor != null)
            {
                return _adaptor.Invoke(collection);
            }

            return collection;
        }

        protected virtual void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            NotifyCollectionChangedEventHandler collectionChanged = CollectionChanged;
            if( collectionChanged != null )
            {
                collectionChanged.Invoke(this, e);
            }
        }

        private List<Lazy<T, M>> AdaptedItems
        {
            get
            {
                if (_adaptedItems == null)
                {
                    _adaptedItems = Adapt(_allItems).ToList();
                }

                return _adaptedItems;
            }
        }

        #region ICollection Implementation
        
        public bool Contains(Lazy<T, M> item)
        {
            return AdaptedItems.Contains(item);
        }

        public void CopyTo(Lazy<T, M>[] array, int arrayIndex)
        {
            AdaptedItems.CopyTo(array, arrayIndex);
        }

        public int Count
        {
            get
            {
                return AdaptedItems.Count;
            }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public IEnumerator<Lazy<T, M>> GetEnumerator()
        {
            return AdaptedItems.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        // Mutation methods work against complete collection
        // and then force a reset of the adapted collection
        public void Add(Lazy<T, M> item)
        {
            _allItems.Add(item);
            ReApplyAdaptor();
        }

        public void Clear()
        {
            _allItems.Clear();
            ReApplyAdaptor();
        }

        public bool Remove(Lazy<T, M> item)
        {
            bool removed = _allItems.Remove(item);
            ReApplyAdaptor();
            return removed;
        }

        #endregion
    }

    public class AdaptingCollection<T> : AdaptingCollection<T, IDictionary<string, object>>
    {
        public AdaptingCollection(Func<IEnumerable<Lazy<T, IDictionary<string, object>>>,
                                       IEnumerable<Lazy<T, IDictionary<string, object>>>> adaptor)
            : base(adaptor)
        { }
    }
}
