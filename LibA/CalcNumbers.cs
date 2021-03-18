using System;
using System.Collections.Generic;

namespace LibA
{
    public class CalcNumbers
    {
        public IEnumerable<int> Calc()
        {
            var prePre = 1;
            var pre = 1;

            yield return prePre;
            yield return pre;

            while (true)
            {
                var actual = prePre + pre;
                prePre = pre;
                pre = actual;

                yield return actual;
                
                if (actual>1000) yield break;
            }
        }
    }
}
