using System.Collections;
using System.Collections.Generic;

namespace Core.Data.Objects.Collections
{
    public class ChildsEntityList<T> : IList<T>
    {
        private string _childPropertyToInverse;
        private object _parentEntity;

        private List<T> _list;

        public ChildsEntityList(object parentEntity, string childPropertyToInverse)
        {
            _childPropertyToInverse = childPropertyToInverse;
            _parentEntity = parentEntity;
            _list = new List<T>();
        }

        public void Add(T obj)
        {
            SetParentToChild(obj);
            _list.Add(obj);
        }
        
        public int IndexOf(T item)
        {
            return _list.IndexOf(item);
        }

        public void Insert(int index, T item)
        {
            SetParentToChild(item);
            _list.Insert(index,item);
        }

        public void RemoveAt(int index)
        {
            _list.RemoveAt(index);
        }

        public T this[int index]
        {
            get { return _list[index]; }
            set
            {
                SetParentToChild(value);
                _list[index] = value;
            }
        }
        
        public void Clear()
        {
           _list.Clear();
        }

        public bool Contains(T item)
        {
            return _list.Contains(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            _list.CopyTo(array, arrayIndex);
        }

        public int Count
        {
            get { return _list.Count; }
        }

        public bool IsReadOnly
        {
            get
            {
                return (bool) _list.GetType().GetProperty("IsReadOnly").GetValue(_list, null);
            }
        }

        public bool Remove(T item)
        {
           return  _list.Remove(item);
        }

        public IEnumerator<T> GetEnumerator()
        {
            return _list.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _list.GetEnumerator();
        }
        
        private void SetParentToChild(T obj)
        {
            typeof(T).GetProperty(_childPropertyToInverse).SetValue(obj, _parentEntity, null);
        }
    }
}
