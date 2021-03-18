using System;
using NUnit.Framework;

namespace TestLibALibB
{
    [TestFixture]
    public class Class1
    {
        [Test]
        public void DoCalcLibA()
        {
            //var sut = new LibA.CalcNumbers();
            //var i = 0;
            //while (i++ < 10)
            //{
            //    Console.WriteLine(sut.Calc());
            //}

            //foreach (var current in sut.Calc())
            //{
            //}
        }


        [Test]
        public void DoCalcLibB()
        {
            var sut = new LibB.CalcNumbers();
            var i = 0;
            while (i++ < 10)
            {
                Console.WriteLine(sut.Calc());
            }
        }

   }
}
