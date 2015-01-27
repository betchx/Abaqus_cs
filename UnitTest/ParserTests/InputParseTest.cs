using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Abaqus;

namespace UnitTest.ParserTests
{
    [TestFixture]
    class InputParseTest : ParserTestBase
    {
        [Datapoints]
        public static IEnumerable<Input> InputData { get { return PartTestBase.data.Take(1); } }
#if false
        
        [Datapoints]
        public static IEnumerable<Input> NoAssemblyInp { get { return InputSample.NotAssemblySample; } }

        [Datapoints]
        public static IEnumerable<Input> WithAssemblyInp { get { return InputSample.WithAssemblySample; } }

#endif

        [Theory]
        public void ValidでないデータはInvalidFormatExceptionをスローする(Input data)
        {
            Assume.That(data.valid, Is.False);
            Assert.Fail(data.Value);
        }

        [Theory]
        public void Validなデータはパース時に例外をスローしない(Input data)
        {
            Assume.That(data, Is.True);

            Assume.That(() => { parser.parse_string(data.Value); }, Throws.Nothing);
        }

        [Theory]
        public void パースに成功すればコレクションを持っている(Input data)
        {
            Assume.That(() => { parser.parse_string(data.Value); }, Throws.Nothing);
            var model = parser.model;
            Assume.That(model, Is.Not.Null);
            Assert.That(model.all_elements, Is.Not.Null);
            Assert.That(model.all_elsets, Is.Not.Null);
            Assert.That(model.all_nodes, Is.Not.Null);
            Assert.That(model.all_nsets, Is.Not.Null);
            Assert.That(model.elements, Is.Not.Null);
            Assert.That(model.elsets, Is.Not.Null);
            Assert.That(model.instances, Is.Not.Null);
            Assert.That(model.nsets, Is.Not.Null);
            Assert.That(model.parts, Is.Not.Null);
        }




        [Theory]
        public void Assemblyがなければpartsは空である(Input data)
        {
            //Assert.Fail();
            Assume.That(data);
            Assume.That(data.Value, Is.Not.Empty);
            var inp = data.Value.ToUpper();

            Assume.That(inp, Is.Not.StringContaining("*" + Keyword.ASSEMBLY));

            var model = parser.parse_string(data.Value);
            Assert.That(model.parts, Is.Empty);
        }

        [Explicit]
        [Theory]
        public void Assemblyがなければinstancesは空である(Input data)
        {
            //Assert.Fail();
            Assume.That(data);
            Assume.That(data.Value, Is.Not.Empty);
            var inp = data.Value.ToUpper();

            Assume.That(inp, Is.Not.StringContaining("*" + Keyword.ASSEMBLY));

            var model = parser.parse_string(data.Value);
            Assert.That(model.instances, Is.Empty);
        }


        [Explicit]
        [Theory]
        public void Assemblyが無くNSETパラメータがあればnsetsは空ではない(Input data)
        {
            //Assert.Fail();
            Assume.That(data.valid,Is.True);
            var inp = data.Value.ToUpper();
            Assume.That(inp, Is.Not.Empty);
            Assume.That(inp, Is.StringContaining("*NSET").Or.StringMatching("NSET +="));
            Assume.That(inp, Is.Not.StringContaining("*" + Abaqus.Keyword.ASSEMBLY));

            var model = parser.parse_string(data.Value);

            Assert.That(model.nsets, Is.Not.Empty);
            Assert.That(model.parts, Is.Empty);
            Assert.That(model.instances, Is.Empty);
        }



        [Explicit]
        [Theory]
        public void NSETパラメータがあればall_nsetsは空ではない(Input data)
        {
            //Assert.Fail();
            Assume.That(data.Value, Is.Not.Empty);
            Assume.That(data.Value.ToUpper(), Is.StringContaining("NSET"));

            var model = parser.parse_string(data.Value);

            Assert.That(model.all_nsets, Is.Not.Empty);
        }

        [Explicit]
        [Theory]
        public void ELSETパラメータが含まれていればELsetは空ではない(Input data)
        {
            //Assert.Fail();
            Assume.That(data.Value, Is.Not.Empty);
            Assume.That(data.Value, Is.StringContaining("ELSET"));

            var model = parser.parse_string(data.Value);

            Assert.That(model.elsets, Is.Not.Empty);
            Assert.That(model.all_elsets, Is.Not.Empty);
        }

        [Explicit]
        [Theory]
        public void Nodeキーワードがあれば節点は存在する(Input data)
        {
            //Assert.Fail();

        }


    }
}
