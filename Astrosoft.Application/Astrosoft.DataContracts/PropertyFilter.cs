using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Astrosoft.DataContracts
{
    public class PropertyFilter
    {
        public readonly string PropertyName;
        public readonly IFilter Filter;

        public PropertyFilter(string propertyName, IFilter filter)
        {
            this.PropertyName = propertyName;
            this.Filter = filter;
        }

    }
}
