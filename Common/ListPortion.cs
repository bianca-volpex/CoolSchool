using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Common
{
    public class ListPortion<T>
    {
        public List<T> items { get; set; }
        public int total { get; set; }

        public ListPortion()
        {
            this.items = new List<T>();
        }

        public ListPortion(List<T> items)
        {
            this.items = items;
            this.total = items.Count;
        }

        public static ListPortion<T> FromList(List<T> items, int offset, int limit)
        {
            return new ListPortion<T>(items.Skip(offset).Take(limit).ToList());
        }
    }

    public class SearchOptions
    {
        public int offset { get; private set; }
        public int limit { get; private set; }
        public string searchText { get; private set; }
        
        public string searchField { get; private set; }
        public string searchType { get; private set; }

        public string excludeField { get; private set; }
        public string excludeText { get; private set; }

        public string sorterField { get; private set; }
        public string sorterOrder { get; private set; }

        public string sorter
        {
            get
            {
                return this.sorterField + " " + this.sorterOrder.ToUpper();
            }
        }

        public SearchOptions(string defaultSortField, string defaultSearch)
        {
            this.sorterField = defaultSortField;
            this.sorterOrder = "asc";
            this.searchField = defaultSearch;
        }

        public SearchOptions(string defaultSortField, string defaultSearch, string defaultSearchType)
        {
            this.sorterField = defaultSortField;
            this.sorterOrder = "asc";
            this.searchField = defaultSearch;
            this.searchType = defaultSearchType;
        }

        public SearchOptions Limit(int offset, int limit)
        {
            this.limit = limit;
            this.offset = offset;
            return this;
        }

        public SearchOptions Sort(string order)
        {
            this.sorterOrder = order;
            return this;
        }

        public SearchOptions Sort(string sorter, string order)
        {
            this.sorterField = sorter;
            this.sorterOrder = order;
            return this;
        }

        public SearchOptions Search(string searchText)
        {
            this.searchText = searchText;
            return this;
        }

        public SearchOptions Search(string searchField, string searchText)
        {
            this.searchField = searchField;
            this.searchText = searchText;
            return this;
        }

        public SearchOptions Exclude(string exField, string exText)
        {
            this.excludeField = exField;
            this.excludeText = exText;
            return this;
        }

        public string Filter()
        {
            string res = "";
            if (!string.IsNullOrEmpty(searchText))
                return res += string.Format(" AND {0} LIKE '%{1}%' ", searchField, searchText);
            if (!string.IsNullOrEmpty(excludeText))
                return res += string.Format(" AND ISNULL({0},'') != '{1}' ", excludeField, excludeText);
            return res;
        }

        public string WithAddSearchFilter(string[]fields)
        {
            string res = "";
            if (!string.IsNullOrEmpty(searchText))
            {
                res += string.Format(" AND ");
                res += string.Format(" (  ");

                res += string.Format(" {0} LIKE '%{1}%' ", searchField, searchText);
                for (var i = 0; i < fields.Length; i++) {
                    res += string.Format(" or {0} LIKE '%{1}%' ", fields[i], searchText);
                }
                res += string.Format(" ) ");

                return res;
            }
            if (!string.IsNullOrEmpty(excludeText))
                return res += string.Format(" AND ISNULL({0},'') != '{1}' ", excludeField, excludeText);
            return res;
        }
    }

}
