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

            Assert.AreEqual(id, elm.id, "ID");

            for (uint i = 0; i < nodes.Length; i++)
			{
                Assert.AreEqual(nodes[i], elm[i], "n{0}", (i+1) );
			}

        }
    }
}
