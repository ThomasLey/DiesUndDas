using System;
using System.Collections.Generic;

namespace LibAB
{
    public class CalcNumbers
    {
        public IEnumerable<int> Calc()
        {
            yield return 2;
            yield return 3;

            var i = 5;
            while (true)
            {
                if (IsPrime(i))
                    yield return i;

                i += 2;
            }
        }

        private static bool IsPrime(int number)
        { 
            if (number <= 1) return false;
            if (number == 2) return true;
            if (number % 2 == 0) return false;

            var boundary = (int)Math.Floor(Math.Sqrt(number));

            for (var i = 3; i <= boundary; i += 2)
                if (number % i == 0)
                    return false;

            return true;
        }
    }
}
