using System;
using System.Collections.Generic;
using System.Linq;

namespace UsesLibB
{
    public class Get10Prime
    {
        public IEnumerable<int> ReturnFirst10()
        {
            return new LibB.CalcNumbers().Calc().Take(10);
        }
    }
}
