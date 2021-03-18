using System;
using System.Linq;
using NUnit.Framework;
using UsesLibA;
using UsesLibB;

namespace WhatUsesAUsesB
{
    [TestFixture]
    public class Class1
    {
        [Test]
        public void DoCalc()
        {
            new Get10Ficonacci().ReturnFirst10().ToList().ForEach(Console.WriteLine);
            new Get10Prime().ReturnFirst10().ToList().ForEach(Console.WriteLine);
        }
    }
}
