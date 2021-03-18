using System;
using System.Collections.Generic;
using System.Linq;

namespace UsesLibA
{
    public class Get10Ficonacci
    {
        public IEnumerable<int> ReturnFirst10()
        {
            return new LibAB.CalcNumbers().Calc().Take(10);
        }
    }
}
