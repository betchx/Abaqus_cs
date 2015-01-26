using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Abaqus;

namespace UnitTest
{
    [TestFixture]
    public class ElementTest
    {

        // 4節点
        static object[] Four =
        {
            new object[] { "S4","1, 1,2,3,4", 1u, new uint[] { 1u, 2u, 3u, 4u} },
        };

        // 8節点
        static object[] Eight =
        {
            new object[] { "C3D8", "2, 1,2,3,4,5,6,7,8", 2u, new uint[]{1,2,3,4,5,6,7,8} },
        };

        [TestCaseSource("Four")]
        [TestCaseSource("Eight")]
        public void SimpleTest(string type, string line, uint id, uint[] nodes)
        {
            var elm = new Element(type, line);

            Assert.AreEqual(nodes.Length, (int)elm.Count());
            Assert.AreEqual(id, elm.id, "ID");

            for (uint i = 0; i < nodes.Length; i++)
            {
                Assert.AreEqual(nodes[i], elm[i], "n{0}", (i+1) );
            }

        }

        [TestCase("B31", Result="B31")]
        [TestCase("S4R", Result="S4R")]
        [TestCase("S4", Result="S4")]
        [TestCase("s4", Result="S4")]
        [TestCase("C3d8", Result="C3D8")]
        [TestCase("c3D20", Result="C3D20")]
        public string ElementTipeTest(string type)
        {
            //Assert.Fail();
            var e = new Element(type, "1, 2, 3");
            return e.type;
        }

        private const string alph = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";
        private const string alnum = "0123456789abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";

        public static IEnumerable<TestCaseData> SourceRondomTypeName
        {
            get {
                var r = new Random();
                for (int i = 0; i < 10; i++) {
                    var len = r.Next(2, 7);
                    var s = new StringBuilder(len);
                    s.Append(alph[r.Next(alph.Length)]);
                    for (int k = 1; k < len; k++) {
                        s.Append(alnum[r.Next(alnum.Length)]);
                    }
                    var str = s.ToString();
                    yield return new TestCaseData(str).Returns(str.ToUpper());
                }
			}
        }

        [TestCaseSource("SourceRondomTypeName")]
        public string RondomTypeNameTest(string type)
        {
            var e = new Element(type,"1, 1, 2, 3, 4");
            return e.type;
        }


        [Test]
        public void ElementSizeTest(
            [Random(1,20,4)] int num
            )
        {
            var line = num.ToString()
                + Enumerable.Range(1, num)
                .Select(n => string.Format(", {0}", n))
                .Aggregate((a, b) => a + b);
            //Assert.Fail(line);
            var el = new Element("DUMMY", line);
            Assert.AreEqual(num, el.Count);
        }
    }
}
