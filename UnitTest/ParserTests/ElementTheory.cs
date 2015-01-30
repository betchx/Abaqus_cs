using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using System.Text.RegularExpressions;
using Abaqus;

namespace UnitTest.ParserTests
{
    class ElementTheory : ParserTestBase
    {
        [Datapoints]
        public IEnumerable<Input> simple_element_definition
        {
            get
            {
                yield return new Input(null);
                yield return new Input("*ELEMENT\n1,1,1\n",false);
                yield return new Input("*element, type=b31, elset=ELM1\n1,1,2\n2,2,3\n");
                yield return new Input("*element, elset=x\n1,1,2\n2,2,3\n",false);
                yield return new Input("*element, type=s4\n1,1,2,3,4\n2,3,4,5,6\n");

            }
        }

        [Theory]
        public void 必須パラメータのない要素定義ではInvalidFormatExceptionがスローされる(Input inp)
        {
            // Assumptions
            Assume.That(inp, Is.Not.Null);
            Assume.That(inp.Value, Is.Not.Null);
            Assume.That(inp.Value, Is.Not.Empty);
            Assume.That(inp.Value, Is.Not.StringMatching("type *=").IgnoreCase);
            // check that assumption is valid
            //Assert.Fail(inp);
            // Work

            // Assertions
            Assert.That(inp.valid, Is.False, string.Format("value : {0}",inp.Value));
            //Assert.That(() => { parser.parse_string(inp); },
            //    Throws.InstanceOf<Abaqus.InvalidFormatException>());
            Assert.Throws<Abaqus.InvalidFormatException>(() => { parser.parse_string(inp.Value); },
                "Nothing was throwed by '" + inp.Value + "'");
        }

        [Theory]
        public void ELSETパラメータがある場合はグローバルの要素集合に同名のセットが追加される(Input inp)
        {
            // Assumptions
            Assume.That(inp, Is.Not.Null);
            Assume.That(inp.Value, Is.Not.Null);
            Assume.That(inp.Value, Is.StringMatching("elset *=").IgnoreCase);
            Assume.That(inp.isValid());
            // check that assumption is valid
            //Assert.Fail(inp);
            // Work

            bool res = false;
            Assert.DoesNotThrow(() => { res = lexer.read_string(inp); });
            Assert.IsTrue(res);
            Assert.DoesNotThrow(() => { parser.parse_string(inp); });

            var elcmds = lexer.commands.Where(c =>  c.keyword == Keyword.ELEMENT);

            // Assertions
            Assert.That(elcmds, Is.Not.Empty);
            Assert.That(parser.model.global_elsets, Is.Not.Empty);
            foreach (var cmd in elcmds) {
                Assert.DoesNotThrow(() => { cmd.must_have("ELSET"); });
                Assert.That(parser.model.global_elsets.Keys, Has.Member(cmd["ELSET"]));
            }
        }
    }
}
