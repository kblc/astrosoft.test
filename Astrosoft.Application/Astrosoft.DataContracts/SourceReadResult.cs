using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Astrosoft.DataContracts
{
    public class SourceReadResult
    {
        public readonly IEnumerable<IItem> Items;
        public readonly long PageCount;
        public readonly long PageIndex;
        public readonly string Source;

        public SourceReadResult(IEnumerable<IItem> items, long pageCount, long pageIndex, string source) {
            this.Items = items;
            this.PageCount = pageCount;
            this.PageIndex = pageIndex;
            this.Source = source;
        }
    }
}
