using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Abaqus;

namespace UnitTest
{
    class IDGeneratorTest
    {
        [Test]
        public void SimpleTest()
        {
            //Assert.Fail();
            var expected = new uint[] { 1, 2, 3, 4, 5 };
            CollectionAssert.AreEqual(expected, new IDGenerater("1, 5, 1"));
        }


        public static IEnumerable<TestCaseData> SourceFirst
        {
            get {
                yield return new TestCaseData("1, 5, 1").Returns(1u);
                yield return new TestCaseData("10, 15, 1").Returns(10u);
                yield return new TestCaseData("20, 30, 5").Returns(20u);
                yield return new TestCaseData("40,50,10").Returns(40u);
            }
        }

        [TestCaseSource("SourceFirst")]
        public uint FirstValueTest(string line)
        {
            return new IDGenerater(line).First();
        }


        public static IEnumerable<TestCaseData> SourceLast
        {
            get {
                yield return new TestCaseData("1, 5, 1").Returns(5);
                yield return new TestCaseData("10, 15, 1").Returns(15u);
                yield return new TestCaseData("20, 30, 5").Returns(30u);
                yield return new TestCaseData("40,50,10").Returns(50u);
                yield return new TestCaseData("40,50,3").Returns(49u);
                yield return new TestCaseData("10,10,1").Returns(10u);
            }
        }

        [TestCaseSource("SourceLast")]
        public uint LastValueTest(string line)
        {
            return new IDGenerater(line).Last();
        }


        public static IEnumerable<TestCaseData> SourceCount
        {
            get {
                yield return new TestCaseData("1,5, 1").Returns(5);
                yield return new TestCaseData("10, 15, 1").Returns(6);
                yield return new TestCaseData("20, 30, 5").Returns(3);
                yield return new TestCaseData("40,50,10").Returns(2);
                yield return new TestCaseData("40,50,3").Returns(4);
                yield return new TestCaseData("1,1,1").Returns(1);
            }
        }

        [TestCaseSource("SourceCount")]
        public int CountTest(string line)
        {
            return new IDGenerater(line).Count();
        }

        public static IEnumerable<TestCaseData> SourceArray
        {
            get {
                yield return new TestCaseData("1,5,1").Returns(new uint[]{ 1u, 2u, 3u, 4u, 5u});
                yield return new TestCaseData("1,5,2").Returns(new uint[]{ 1u, 3u, 5u});
                yield return new TestCaseData("10, 20, 10").Returns(new uint[]{10u, 20u});

            }
        }

        [TestCaseSource("SourceArray")]
        public uint[] ArrayTest(string line)
        {
            return new IDGenerater(line).ToArray();
        }

    }
}
