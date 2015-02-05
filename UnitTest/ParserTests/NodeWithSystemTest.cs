using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;


namespace UnitTest.ParserTests
{
    class NodeWithSystemTest : ParserTestBase
    {
        [Test]
        public void SimpleTranslateTest()
        {
            //Assert.Fail("SimpleTranslateTest");
            var inp = @"*SYSTEM
1,1,1
*NODE
1, 1, 2,3
";
            var m = parser.parse_string(inp);

            Assert.That(parser.system.Children, Is.Not.Empty);

            var n = m.nodes[1u];
            Assert.AreEqual(2.0, n.X, 0.001);
            Assert.AreEqual(3.0, n.Y, 0.001);
            Assert.AreEqual(4.0, n.Z, 0.001);
        }
    }
}
