using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Abaqus;

namespace UnitTest.ParserTests
{
    class NodeTheory : ParserTestBase
    {

        [Datapoints]
        public IEnumerable<Input> node_definition
        {
            get
            {
                yield return new Input("*NODE\n1\n");
                yield return new Input("*node,nset=all\n2\n");
                yield return new Input("*node\n1,2,3\n,4,5,6\n");
                yield return new Input("*node\n**\n1,6.0, 4.5, 1.3");
            }
        }

        [Theory]
        public void 有効な文字列をパースしたあとはGlobalNodeにItemが存在する(Input inp)
        {
            Assume.That(inp, Is.Not.Null);
            Assume.That(inp.valid);
            //Assert.Fail();
            Assert.That(parser.model.global_nodes, Is.Not.Null.Or.Empty);
        }
    }
}
