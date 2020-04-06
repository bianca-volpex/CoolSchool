using System;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Common
{
    public static class EFHelper
    {
        public static IQueryable<T> ApplyFilterByType<T>(this IQueryable<T> source, SearchOptions options)
        {
            bool noFilter = string.IsNullOrEmpty(options.searchText);
            IQueryable<T> q = source;
            if (!noFilter)
            {
                switch (options.searchType)
                {
                    case "fullInt":
                        q = q.Where(options.searchField + ".ToString() == @0", options.searchText);
                        break;
                    case "int":
                        q = q.Where(options.searchField + ".ToString().Contains(@0)", options.searchText);
                        break;
                    case "date":
                        var date = DateTime.Parse(options.searchText);
                        var dateEnd = date.AddDays(1).AddMilliseconds(-1);

                        q = q
                            .Where(options.searchField + " >= @0", date)
                            .Where(options.searchField + " <= @0", dateEnd);

                        break;
                    default:
                        var fields = options.searchField.Split(';');
                        var sf = "";
                        for (var i = 0; i < fields.Length; i++) {
                            fields[i] = string.Format("@{0}.Contains(@0)", fields[i]);
                        }
                         sf = string.Join(" or ", fields);
                         q = q.Where(sf, options.searchText);

                        break;
                }
            }

            var items = q.OrderBy(options.sorter);

            return items;
        }

        public static IQueryable<T> ApplyPaging<T>(this IQueryable<T> source, SearchOptions options)
        {
            var items = source.Skip(options.offset).Take(options.limit);
            return items;
        }
    }
}
