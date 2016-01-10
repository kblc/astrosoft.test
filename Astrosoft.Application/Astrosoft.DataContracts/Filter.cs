using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Astrosoft.DataContracts
{
    public class Filter : IFilter
    {
        private Filter(Func<object,bool> filterFunc)
        {
            if (filterFunc == null)
                throw new ArgumentNullException(nameof(filterFunc));
            this.filterFunction = filterFunc;
        }

        private readonly Func<object, bool> filterFunction;

        private static readonly Func<object, object, bool> isEqualsFunction = (o1,o2) => (o1 != null && o1.Equals(o2)) || (o1 == null && o2 == null);
        private static readonly Func<string, string, bool> isContainsFunction = (o1, o2) => (o1 != null && o1.Contains(o2)) || (o1 == null && o2 == null);
        private static readonly Func<IComparable, IComparable, bool> isMoreThanFunction = (o1, o2) => (o1 != null && o1.CompareTo(o2) > 0);
        private static readonly Func<IComparable, IComparable, bool> isLessThanFunction = (o1, o2) => (o2 != null && o2.CompareTo(o1) > 0);

        public static Filter GetFilterContains(object filterValue)
        {
            return new Filter(o => isContainsFunction(o as string, filterValue as string));
        }

        public static Filter GetFilterEquals(object filterValue)
        {
            return new Filter(o => isEqualsFunction(o, filterValue));
        }

        public static Filter GetFilterNotEquals(object filterValue)
        {
            return new Filter(o => !isEqualsFunction(o, filterValue));
        }

        public static Filter GetFilterStartFrom(IComparable filterValue)
        {
            return new Filter(o => isMoreThanFunction((IComparable)o, filterValue));
        }

        public static Filter GetFilterEndTo(IComparable filterValue)
        {
            return new Filter(o => isLessThanFunction((IComparable)o, filterValue));
        }

        public static Filter GetFilterBetween(IComparable filterValueMin, IComparable filterValueMax)
        {
            return new Filter(o =>
                isMoreThanFunction((IComparable)o, filterValueMin) &&
                isLessThanFunction((IComparable)o, filterValueMax));
        }

        public bool isFiltered(object value)
        {
            return filterFunction(value);
        }
    }
}
