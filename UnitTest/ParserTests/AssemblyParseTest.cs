using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Abaqus;

namespace UnitTest.ParserTests
{
    [TestFixture]
    public class AssemblyTest : PartTestBase
    {


        [Sequential]
        public void AllNSetsNameTest(
            [ValueSource(typeof(PartTestBase), "data")] Input data,
            [ValueSource(typeof(PartTestBase), "nset_names")] Name set_name)
        {
            var model = parser.parse_string(data);
            var all = model.global_nsets;

            CollectionAssert.Contains(all.Keys, set_name.Value);
        }


        [Combinatorial]
        public void IDTest(
            [ValueSource(typeof(PartTestBase), "data")]  Input data,
            [ValueSource(typeof(PartTestBase), "ids")] uint id)
        {
            var model = parser.parse_string(data);
            var all = model.global_nsets;

            Assert.That(all, Is.Not.Empty);
            var name = all.First().Key;
            Assert.That(name, Is.Not.Empty);
            Assert.That(name, Is.StringContaining("."));

            var instance_name = name.Split('.').First();

            Assert.IsNotEmpty(instance_name);

            CollectionAssert.Contains(all.Values.First(), new Address(instance_name, id));
        }

        [Test]
        public void ModelTestOfParts([ValueSource(typeof(PartTestBase), "data")] Input data)
        {
            //Assert.Fail();
            var model = parser.parse_string(data);
            model.parts.ForEach(kv => Assert.AreEqual(model, kv.Value.model));
        }

        [Test]
        public void ModelTestOfNsets([ValueSource(typeof(PartTestBase), "data")] Input data)
        {
            //Assert.Fail();
            var model = parser.parse_string(data);
            model.nsets.ForEach(kv => Assert.AreEqual(model, kv.Value.model));
        }

        [Test]
        public void ModelTestOfNodes([ValueSource(typeof(PartTestBase), "data")] Input data)
        {
            //Assert.Fail();
            var model = parser.parse_string(data);
            model.nodes.ForEach(kv => Assert.AreEqual(model, kv.Value.model));
        }

        [Test]
        public void ModelTestOfInstances([ValueSource(typeof(PartTestBase), "data")] Input data)
        {
            //Assert.Fail();
            var model = parser.parse_string(data);
            model.instances.ForEach(kv => Assert.AreEqual(model, kv.Value.model));
        }

        [Test]
        public void ModelTestOfElsets([ValueSource(typeof(PartTestBase), "data")] Input data)
        {
            //Assert.Fail();
            var model = parser.parse_string(data);
            model.elsets.ForEach(kv => Assert.AreEqual(model, kv.Value.model));
        }

        [Test]
        public void ModelTestOfElements([ValueSource(typeof(PartTestBase), "data")] Input data)
        {
            //Assert.Fail();
            var model = parser.parse_string(data);
            model.elements.ForEach(kv => Assert.AreEqual(model, kv.Value.model));
        }

        [Test]
        public void ModelTestOfGlobalNodes([ValueSource(typeof(PartTestBase), "data")] Input data)
        {
            //Assert.Fail();
            var model = parser.parse_string(data);
            model.global_nodes.ForEach(kv => Assert.AreEqual(model, kv.Value.model));
        }

        [Test]
        public void ModelTestOfGlobalElsets([ValueSource(typeof(PartTestBase), "data")] Input data)
        {
            //Assert.Fail();
            var model = parser.parse_string(data);
            model.global_elsets.ForEach(kv => Assert.AreEqual(model, kv.Value.model));
        }

        [Test]
        public void ModelTestOfGlobalNsets([ValueSource(typeof(PartTestBase), "data")] Input data)
        {
            //Assert.Fail();
            var model = parser.parse_string(data);
            model.global_nsets.ForEach(kv => Assert.AreEqual(model, kv.Value.model));
        }

        [Test]
        public void ModelTestOfGlobalElements([ValueSource(typeof(PartTestBase), "data")] Input data)
        {
            //Assert.Fail();
            var model = parser.parse_string(data);
            model.global_elements.ForEach(kv => Assert.AreEqual(model, kv.Value.model));
        }


        [Test]
        public void ParentTest([ValueSource(typeof(PartTestBase), "data")] Input data)
        {
            //Assert.Fail();
            Assume.That(data.Value, Is.Not.Null.Or.Empty);
            var model = parser.parse_string(data);
            Assert.IsNotNull(model);
            Assert.IsNull(model.parent);

            model.elements.ForEach(kv => Assert.AreSame(model.elements, kv.Value.parent));
            model.elsets.ForEach(kv => Assert.AreSame(model.elsets, kv.Value.parent));
            model.instances.ForEach(kv => Assert.AreSame(model.instances, kv.Value.parent));
            model.nodes.ForEach(kv => Assert.AreSame(model.nodes, kv.Value.parent));
            model.nsets.ForEach(kv => Assert.AreSame(model.nsets, kv.Value.parent));
            model.parts.ForEach(kv => Assert.AreSame(model.parts, kv.Value.parent));
        }
    }

}
