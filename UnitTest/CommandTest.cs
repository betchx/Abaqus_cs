using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Abaqus;

namespace UnitTest
{
    class CommandTest
    {

        [Datapoint]
        public static Command lowercase = new Command("node");

        [Datapoint]
        public static Command assembry = new Command(Abaqus.Keyword.ASSEMBLY);


        [Datapoints]
        public static IEnumerable<Command> command_sample
        {
            get
            {
                // 小文字
                var cmd = new Command("element");
                yield return cmd;

                // 大文字
                cmd.keyword = Abaqus.Keyword.ASSEMBLY;
                yield return cmd;

                // ELSET
                var elset = new Command(Keyword.ELSET);
                elset.parameters.Add("ELSET", "ALL");
                elset.datablock.Add("1, 2, 3");
                yield return elset;

                // Element
                var elm = new Command(Keyword.ELEMENT);
                elm.parameters.Add("TYPE", "S4R");
                elm.parameters.Add("ELSET", "SHELL");
                elm.datablock.Add("1, 1, 2, 3, 4");
                yield return elm;

                // Node
                var node = new Command(Keyword.NODE);
                node.datablock.Add("1");
                yield return node;
                node.datablock.Add("2,1");
                yield return node;
                node.datablock.Add("3,2.,3.");
                yield return node;
                node.datablock.Add("4,3.,3.,1");
                yield return node;

                // 存在しないキーワード
                var not_exist = new Command("hogehoge");
                not_exist.datablock.Add("not exist");
                yield return not_exist;
                not_exist.parameters.Add("FOO", "bar");
                yield return not_exist;
                not_exist.parameters.Add("part", "");
                yield return not_exist;

            }
        }

        [Datapoints]
        public static string[] elset_params = new string[]{"elset", "NSET", "type", "name", "part"};


        [Theory]
        public void CommandMustHaveNotEmptyKeyword(Command cmd)
        {
            Assert.That(cmd.keyword, Is.Not.Null.Or.Empty);
        }

        [Theory]
        public void KeywordMustBeUppercase(Command cmd)
        {
            Assert.That(cmd.keyword, Is.EqualTo(cmd.keyword.ToUpper()));
        }

        [Theory]
        public void NoParameterCommandReturnFalseByHasMethodForAnyInputs(Command cmd, string param)
        {
            Assume.That(cmd.parameters.Keys, Is.Empty.Or.Not.Contains(param.ToUpper()));
            Assert.That(cmd.Has(param), Is.False);
        }


        [Theory]
        public void CommandWithParameterReturnsTrueWhenHasMethodCalledWithCorrectArgs(Command cmd, string param)
        {
            Assume.That(cmd.parameters.Keys, Has.Member(param.ToUpper()));
            //Assert.Fail();
            Assert.That(cmd.Has(param), Is.True);
        }


        [Theory]
        public void CommandIsNotEmptyIftheDatablockHasAnyItems(Command cmd)
        {
            Assume.That(cmd.datablock, Is.Not.Empty);
            //Assert.Fail();
            Assert.That(cmd, Is.Not.Empty);
        }

        [Theory]
        public void CommandIndexMayReturnStringIfDatabblockHasAnyItem(Command cmd)
        {
            Assume.That(cmd.datablock, Is.Not.Empty);
            //Assert.Fail();
            string s;
            Assert.That(() => { s = cmd.First(); }, Throws.Nothing);
            Assert.That(cmd.First(), Is.InstanceOf<string>());
        }

        [Theory]
        public void MissingReturnNegateOfHas(Command cmd, string param)
        {
            //Assert.Fail();
            Assert.That(cmd.Missing(param), Is.Not.EqualTo(cmd.Has(param)));
        }

        [Theory]
        public void must_beThowsArgumentExceptionIfArgumentIsNotEqualToKeyword(Command cmd, string keyword)
        {
            Assume.That(cmd.keyword, Is.Not.EqualTo(keyword.ToUpper()));
            //Assert.Fail();
            Assert.That(new TestDelegate(() => cmd.must_be(keyword)), Throws.TypeOf<ArgumentException>());
        }

        [Theory]
        public void must_beMustNotThrowIfArgumentIsEqualToItsKeyword(Command cmd, string keyword)
        {
            Assume.That(cmd.keyword, Is.EqualTo(keyword.ToUpper()));
            //Assert.Fail();
            Assert.That(new TestDelegate(()=>cmd.must_be(keyword)), Throws.Nothing);
        }


        [Theory]
        public void must_haveは所定の名前のパラメータを有する場合例外をスローしない(
            Command cmd,
            string param_name)
        {
            Assume.That(cmd.parameters.Keys, Has.Member(param_name.ToUpper()));
            //Assert.Fail();
            Assert.DoesNotThrow(() => {cmd.must_have(param_name);});
        }

        [Theory]
        public void must_haveは所定の名前のパラメータを有しない場合InvalidFormatExceptionをスローする(
            Command cmd,
            string param_name)
        {
            Assume.That(cmd.parameters.Keys, Has.No.Member(param_name.ToUpper()));
            //Assert.Fail();
            Assert.Throws<InvalidFormatException>(() => { cmd.must_have(param_name); });
        }

    }
}
