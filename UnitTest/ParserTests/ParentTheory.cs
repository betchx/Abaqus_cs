using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Abaqus;

namespace UnitTest.ParserTests
{
    public class ParentTheory : PartTestBase
    {

        [Datapoints]
        public IEnumerable<Input> Data { get { return PartTestBase.data; } }


        [Theory]
        public void ParentOfGlobalElSetItemsShouldBeNull(Input data)
        {
            //Assert.Fail();
            Assume.That(data.Value, Is.Not.Null.Or.Empty);
            var model = parser.parse_string(data);
            model.global_elsets.ForEach(kv => Assert.That(kv.Value.parent, Is.Null));
        }

        [Theory]
        public void ParentOfGlobalNSetsItemsShouldBeNull(Input data)
        {
            //Assert.Fail();
            Assume.That(data.Value, Is.Not.Null.Or.Empty);
            var model = parser.parse_string(data);
            model.global_nsets.ForEach(kv => Assert.That(kv.Value.parent, Is.Null));
        }

        [Theory]
        public void ParentOfGlobalElementItemShouldNotBeNullAndSelf(Input data)
        {
            //Assert.Fail();
            Assume.That(data.Value, Is.Not.Null.Or.Empty);
            var model = parser.parse_string(data);

            // 要素は共通の為親は一致しない．
            model.global_elements.ForEach(kv => Assert.That(kv.Value.parent, Is.Not.Null));
            model.global_elements.ForEach(kv => Assert.That(kv.Value.parent, Is.Not.SameAs(model.global_elements)));
        }

        [Theory]
        public void ParentOfAllNodesItemsShouldNotBeNullAndSelf(Input data)
        {
            //Assert.Fail();
            Assume.That(data.Value, Is.Not.Null.Or.Empty);
            var model = parser.parse_string(data);
            // 節点は共通の為親は一致しない．
            model.global_nodes.ForEach(kv => Assert.That(kv.Value.parent, Is.Not.Null));
            model.global_nodes.ForEach(kv => Assert.That(kv.Value.parent, Is.Not.SameAs(model.global_nodes)));
        }

    }
}
