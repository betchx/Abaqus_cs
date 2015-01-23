using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Abaqus;
using System.IO;

namespace UnitTest.ParserTests
{
    [TestFixture]
    public class ParserTest
    {
        [Test]
        public void DummyTest()
        {
            Assert.True(true);
        }

        Parser parser;
        string file;

        [SetUp]
        public void setup()
        {
            parser = new Parser();
            file = Path.GetTempFileName();
        }

        [TearDown]
        public void teardown()
        {
            File.Delete(file);
        }

        [Test]
        public void ParseElementTest()
        {
            using (var f = new StreamWriter(file))
            {
                f.WriteLine("*ELEMENT, type=S4");
                f.WriteLine("1, 1, 2, 3, 4");
                f.WriteLine("4, 4, 5, 6, 7");
            }
            var model = parser.parse_file(file);
            var elements = model.elements;
            var all = model.all_elements;

            Assert.AreEqual(2, elements.Count, "Number of elements");

            Assert.Contains(1u, elements.Keys);
            var e1 = elements[1u];
            Assert.AreEqual(1u, e1.id);
            var n1 = new uint[] { 1u, 2u, 3u, 4u };
            for (uint i = 0; i < 4u; i++)
            {
                Assert.AreEqual(n1[i], e1[i]);
            }
            Assert.Contains(4u, elements.Keys);
            var e4 = elements[4u];
            Assert.AreEqual(4u, e4.id);
            var n4 = new uint[] { 1u, 2u, 3u, 4u };
            for (uint i = 0; i < 4u; i++)
            {
                Assert.AreEqual(n4[i], e4[i]);
            }
        }


        [Test]
        public void ParseNodeTest()
        {
            Assert.NotNull(file);
            using (var f = new StreamWriter(file))
            {
                f.WriteLine("*NODE");
                f.WriteLine("1, 1.0, 2.0, 3.0");
                f.WriteLine("2, 1.0, 2.0");
                f.WriteLine("3, 1.0");
                f.WriteLine("4, 4., 5., 6.");
            }
            parser.parse_file(file);
            var model = parser.model;
            var nodes = model.nodes;
            var all = model.all_nodes;


            for (uint i = 1u; i < 5u; i++)
            {
                Assert.True(nodes.ContainsKey(i),"nodes");
                Assert.True(all.ContainsKey(new Address(i)), "all_nodes");
                Assert.AreSame(nodes[i], all[new Address(i)], "all");
                Assert.NotNull(nodes[i].parent);
                Assert.AreSame(model, nodes[i].model,"Model");
            }

            const double d = 0.001; // delta
 
            var n1 = nodes[1u];
            Assert.AreEqual(1u, n1.id, "n1.ID");
            Assert.AreEqual(1.0, n1.X, d, "n1.X");
            Assert.AreEqual(2.0, n1.Y, d, "n1.Y");
            Assert.AreEqual(3.0, n1.Z, d, "n1.Z");

            var n2 = nodes[2u];
            Assert.AreEqual(2u, n2.id, "n2.ID");
            Assert.AreEqual(1.0, n2.X, d, "n2.X");
            Assert.AreEqual(2.0, n2.Y, d, "n2.Y");
            Assert.AreEqual(0.0, n2.Z, d, "n2.Z");

            var n3 = nodes[3u];
            Assert.AreEqual(3u, n3.id, "n3.ID");
            Assert.AreEqual(1.0, n3.X, d, "n3.X");
            Assert.AreEqual(0.0, n3.Y, d, "n3.Y");
            Assert.AreEqual(0.0, n3.Z, d, "n3.Z");

            var n4 = nodes[4u];
            Assert.AreEqual(4u, n4.id, "n4.ID");
            Assert.AreEqual(4.0, n4.X, d, "n4.X");
            Assert.AreEqual(5.0, n4.Y, d, "n4.Y");
            Assert.AreEqual(6.0, n4.Z, d, "n4.Z");

        }


    }


}
