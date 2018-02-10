
using System.Collections;
using System.Collections.Generic;

namespace PodHead
{
    internal class ConcurrentList<T> : IList<T>
    {
        private readonly IList<T> _list = new List<T>();

        private static readonly object _lock = new object();

        public T this[int index]
        {
            get
            {
                lock(_lock)
                {
                    return _list[index];
                }
            }
            set
            {
                lock(_lock)
                {
                    _list[index] = value;
                }
            }
        }

        public int Count
        {
            get
            {
                lock(_lock)
                {
                    return _list.Count;
                }
            }
        }

        public bool IsReadOnly
        {
            get
            {
                lock(_lock)
                {
                    return _list.IsReadOnly;
                }
            }
        }

        public void Add(T item)
        {
            lock(_lock)
            {
                _list.Add(item);
            }
        }

        public void Clear()
        {
            lock(_lock)
            {
                _list.Clear();
            }
        }

        public bool Contains(T item)
        {
            lock(_lock)
            {
                return _list.Contains(item);  
            }
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            lock (_lock)
            {
                _list.CopyTo(array, arrayIndex);
            }
        }

        public IEnumerator<T> GetEnumerator()
        {
            lock (_lock)
            {
                return _list.GetEnumerator();
            }
        }

        public int IndexOf(T item)
        {
            lock (_lock)
            {
                return _list.IndexOf(item);
            }
        }

        public void Insert(int index, T item)
        {
            lock (_lock)
            {
                _list.Insert(index, item);
            }
        }

        public bool Remove(T item)
        {
            lock (_lock)
            {
                return _list.Remove(item);
            }
        }

        public void RemoveAt(int index)
        {
            lock (_lock)
            {
                _list.RemoveAt(index);
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void AddRange(IEnumerable<T> source)
        {
            foreach(T item in source)
            {
                Add(item);
            }
        }
    }
}
