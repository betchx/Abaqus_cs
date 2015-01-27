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
        public void AllElSetParentTest(Input data)
        {
            //Assert.Fail();
            Assume.That(data.Value, Is.Not.Null.Or.Empty);
            var model = parser.parse_string(data);
            model.all_elsets.ForEach(kv => Assert.AreSame(model.all_elsets, kv.Value.parent));
        }

        [Theory]
        public void AllNSetsParentTest(Input data)
        {
            //Assert.Fail();
            Assume.That(data.Value, Is.Not.Null.Or.Empty);
            var model = parser.parse_string(data);
            model.all_nsets.ForEach(kv => Assert.AreSame(model.all_nsets, kv.Value.parent));
        }

        [Theory]
        public void AllElementParentTest(Input data)
        {
            //Assert.Fail();
            Assume.That(data.Value, Is.Not.Null.Or.Empty);
            var model = parser.parse_string(data);

            // 要素は共通の為親は一致しない．
            model.all_elements.ForEach(kv => Assert.That(kv.Value.parent, Is.Not.SameAs(model.all_elements)));
        }

        [Theory]
        public void AllNodeParentTest(Input data)
        {
            //Assert.Fail();
            Assume.That(data.Value, Is.Not.Null.Or.Empty);
            var model = parser.parse_string(data);
            // 節点は共通の為親は一致しない．
            model.all_nodes.ForEach(kv => Assert.That(kv.Value.parent, Is.Not.SameAs(model.all_nodes)));
        }

    }
}
