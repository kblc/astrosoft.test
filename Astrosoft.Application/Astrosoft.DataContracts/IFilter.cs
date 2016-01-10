using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Astrosoft.DataContracts
{
    public interface IFilter
    {
        bool isFiltered(object value);
    }
}
