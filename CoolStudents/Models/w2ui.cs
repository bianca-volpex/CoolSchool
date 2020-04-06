using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common;

namespace CoolStudents.Models
{
    public class w2ui
    {
        public class SearchItem
        {
            public string field { get; set; }
            public string type { get; set; }
            public string value { get; set; }
            public string @operator { get; set; }
        }

        public class SortItem
        {
            public string field { get; set; }
            public string direction { get; set; }
        }

        public class GridFilterModel
        {
            public int limit { get; set; }
            public int offset { get; set; }
            public string searchLogic { get; set; }
            public SearchItem[] search { get; set; }
            public SortItem[] sort { get; set; }

            public SearchOptions getOptions(string sortfield, string searchField)
            {
                return this.getOptions(new SearchOptions(sortfield, searchField));
            }

            public SearchOptions getOptions(SearchOptions defaults)
            {
               SearchOptions res = defaults;

                if (search != null && search.Length > 0)
                {
                    res = res.Search(search[0].field, search[0].value);
                }

                if (sort != null && sort.Length > 0)
                {
                    res = res.Sort(sort[0].field, sort[0].direction);
                }

                return res.Limit(this.offset, this.limit);
            }
        }

        public class w2uiGridJsonPostModel : GridFilterModel
        {
            public string editRecord { get; set; }
            public object postData { get; set; }

            public string cmd { get; set; }
            public string[] selected { get; set; }

            public Dictionary<string,
                Dictionary<string, string>
                > changes
            { get; set; }

            public string defaultSearch()
            {
                if (this.search == null || this.search.Length == 0)
                    return null;
                else return this.search[0].value;
            }

            public string defaultSearch(string field)
            {
                if (this.search == null || this.search.Length == 0)
                    return null;
                else
                {
                    var st = this.search.FirstOrDefault(f => f.field == field);
                    if (st != null) return st.value;
                    else return null;
                }
            }
        }

        public class w2uiBaseModel
        {
            public string status { get; set; } //  : "success", error
            public string message { get; set; }
            public object result { get; set; }
            public string trace { get; set; }

            public w2uiBaseModel()
            {
                this.status = "success";
            }

            public static w2uiBaseModel Error(string message)
            {
                return new w2uiBaseModel { status = "error", message = message };
            }

            public static w2uiBaseModel Error(Exception ex)
            {
                return new w2uiBaseModel { status = "error", message = ex.Message, trace = ex.StackTrace };
            }

            public static w2uiBaseModel Ok()
            {
                return new w2uiBaseModel();
            }

            public static w2uiBaseModel Ok(string message)
            {
                return new w2uiBaseModel { status = "success", message = message };
            }

            public static w2uiBaseModel Ok(string message, object result)
            {
                return new w2uiBaseModel { status = "success", message = message, result = result };
            }
        }

        public class w2uiBaseModel<T> : w2uiBaseModel
        {
            public T data { get; set; }
            public w2uiBaseModel(T value) : base()
            {
                this.data = value;
            }
        }

        public class w2uiJsonFormPostModel<T> where T : new()
        {
            public string cmd { get; set; }
            public int recid { get; set; }
            public T record { get; set; }
        }

        public class w2uiBaseGridModel : w2uiBaseModel
        {
            public int total { get; set; } //   : 36,
            public w2uiBaseGridModel() : base() { }
        }

        public class w2uiGridModel<T> : w2uiBaseGridModel
        {
            public List<T> records { get; set; }

            public w2uiGridModel() : base()
            {
                this.records = new List<T>();
            }

            public w2uiGridModel(List<T> items)
                : base()
            {
                this.records = items;
                this.total = items.Count;
            }

            public w2uiGridModel(ListPortion<T> items)
            {
                this.records = items.items;
                this.total = items.total;
            }
        }

        public class w2uiItemsModel<T> : w2uiBaseModel
        {
            public List<T> items { get; set; }

            public w2uiItemsModel()
            {
                this.items = new List<T>();
            }
        }
    }
}
