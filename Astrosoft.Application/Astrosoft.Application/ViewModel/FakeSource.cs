using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Astrosoft.Application.ViewModel
{
    public class FakeSource : DataContracts.ModelSource
    {
        public FakeSource() : base(new Astrosoft.FakeSource.FakeSource(920), 100) { }
    }
}
