using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Abaqus;

namespace UnitTest
{
    [TestFixture]
    public class NodeTest
    {
        const double d = 0.001;

        [TestCase("1, 1.0, 2.0, 3.0",
            1u, 1.0, 2.0, 3.0, d)]
        [TestCase("2, 1.0, 2.0",
            2u, 1.0, 2.0, 0.0, d)]
        [TestCase("3, 1.0",
            3u, 1.0, 0.0, 0.0, d)]
        [TestCase("4, 1., 2., 3.",
            4u, 1.0, 2.0, 3.0, d)]
        public void SimpleTest(string data, uint id, double x, double y, double z, double delta)
        {
            var node = new Node(data);
            Assert.AreEqual(id, node.id, "ID");
            Assert.AreEqual(x, node.X, delta, "X");
            Assert.AreEqual(y, node.Y, delta, "Y");
            Assert.AreEqual(z, node.Z, delta, "Z");
        }

    }
}
