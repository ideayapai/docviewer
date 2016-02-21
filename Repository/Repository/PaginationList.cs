using System;
using System.Collections;
using System.Collections.Generic;

namespace Repository
{
    /// <summary>
    /// 分页基类
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class PaginationList<T> : IPagination<T>, IEnumerable<T>
    {
        private readonly List<T> _items;

        public PaginationList()
        {
            _items = new List<T>();
        }

        public PaginationList(int capacity)
        {
            _items = new List<T>(capacity);
        }

        public PaginationList(IEnumerable<T> collection)
        {
            _items = new List<T>(collection);
        }

        public int TotalPages
        {
            get { return (int) Math.Ceiling((double) TotalCount/PageSize); }
        }

        public List<T> Items
        {
            get { return _items; }
        }

        public T this[int index]
        {
            get { return _items[index]; }
        }

        public int TotalCount { get; set; }
        public int Index { get; set; }
        public int PageSize { get; set; }

        public bool HasNext
        {
            get { return Index < TotalPages; }
        }

        public bool HasPrevious
        {
            get { return Index >= TotalPages; }
        }


        public IEnumerator<T> GetEnumerator()
        {
            return _items.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _items.GetEnumerator();
        }
    }

    public interface IPagination<out T> : IEnumerable<T>
    {
        int TotalPages { get; }

        int TotalCount { get; set; }

        int Index { get; set; }

        int PageSize { get; set; }

        bool HasNext { get; }

        bool HasPrevious { get; }

    }
}
