using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Abaqus;
using System.Text.RegularExpressions;

namespace UnitTest
{
    [TestFixture]
    public class LexTest
    {
        Lexer lexer;

        [SetUp]
        public void setupLex()
        {
            lexer = new Lexer();
        }


        public enum Target
        {
            Keyword,
            Data,
        }

        public static IEnumerable<TestCaseData> LexTargets
        {
            get {
                string key1 = "NODE";
                string data1 = "1, 1.0, 2.0";
                string node_1 = "*"+key1+"\n"+data1;
                yield return new TestCaseData(node_1, 0, Target.Keyword, 0).Returns(key1);
                yield return new TestCaseData(node_1, 0, Target.Data, 0).Returns(data1);
                yield return new TestCaseData(node_1, 0, Target.Data, 1).Throws(typeof(ArgumentOutOfRangeException));
            }
        }


        [Test, TestCaseSource("LexTargets")]
        public string lex_string_test(string data, int command_index, Target target, int data_index)
        {
            lexer.read_string(data);
            var cmd = lexer.commands.ElementAt(command_index);
            switch (target)
            {
                case Target.Keyword:
                    return cmd.keyword;
                case Target.Data:
                    return cmd.datablock[data_index];
                default:
                    throw new ArgumentException("Invalid Target");
            }
        }

        public static string input = @"*HEADING
hogehoge
** COMMENT
*NODE
1, 1., 1,
2, 3., 2,
3, 4.
*ELEMENT, TYPE=B31, ELSET=bar
1, 1, 2
2, 2, 3
*BEAM SECTION, SECTION=R, MATERIAL=STEEL
0.2
0, 0, -1
*MATERIAL, NAME=STEEL
*ELASTIC
2e8, 0.3
*DENSITY
7.8
*NSET, NSET=ENDS
1, 3
*NSET, NSET=MID
2
*BOUNDARY
ENDS, 1, 3
*STEP
Test step
*STATIC
0.1, 1.0
*CLOAD, OP=NEW
2, 0, -1, 0, 0.1
*OUTPUT, FIELD
*NODE OUTPUT, NSET=MID
U2,
*ELEMENT OUTPUT
SF
*END STEP
";
        static int[] data_counts = {1, 3, 2, 2, 0, 1, 1, 1, 1, 1, 1, 1, 1, 0, 1, 1, 0 };
        static int[] option_counts = {0, 0, 2, 2, 1, 0, 0, 1, 1, 0, 0, 0, 1, 1, 1, 0, 0};

        [Test]
        public void KeywordsTest()
        {
            var keywords = input.Split('\n')
                .Where(s => s.StartsWith("*"))
                .Where(s => !s.StartsWith("**"))
                .Select(s => s.Substring(1).Split(',').First().Trim().ToUpper());
            lexer.read_string(input);
            var found_keys = lexer.commands.Select(c => c.keyword);
            CollectionAssert.AreEqual(keywords.ToArray(), found_keys.ToArray());
        }

        [Test]
        public void DataSizeTest()
        {
            lexer.read_string(input);
            var data_sizes = lexer.commands.Select(c => c.datablock.Count);
            CollectionAssert.AreEqual(data_counts, data_sizes);
        }

        [Test]
        public void ParameterSizeTest()
        {
            lexer.read_string(input);
            var param_sizes = lexer.commands.Select(c => c.parameters.Count);
            CollectionAssert.AreEqual(option_counts, param_sizes);
        }

        [Test]
        public void SetsTest()
        {
            lexer.read_string(input);
            Assert.AreEqual(2, lexer.commands.Where(c => c.keyword == "NSET").Count());
            Assert.AreEqual(0, lexer.commands.Where(c => c.keyword == "ELSET").Count());
        }


        public class LexTheroy
        {
            [SetUp]
            public void Setup()
            {
                lex = new Lexer();
            }

            private Lexer lex;

            [Datapoints]
            static IEnumerable<string> inputs()
            {
                yield return "*NODE\n1\n";
                yield return "*NODE\n1, 1.0\n";
                yield return "*NODE\n1, 1.0, 1.0\n";
                yield return "*NODE\n1, 1.0, 1.0, 1.0\n";
                yield return "*NODE\n1, 1,1,1\n2,2,2,2\n";
                yield return "*NODE\n1\n2\r\n*ELEMENT,TYPE=B31\n1, 1,2\n";

                // 存在しないキーワード
                yield return "*WRONG KEYWORD, ANYPARAM=VALUE\nTrailing data line\n";

                // フルセット
                yield return LexTest.input;
            }


            [Theory]
            public void LexParserShouldAddCommand(string line)
            {
                Assume.That(line, Is.StringMatching(@"^\*[^* ][a-zA-Z]"));
                lex.read_string(line);
                Assert.That(lex.commands, Is.Not.Empty);
            }

            [Theory]
            public void NumberOfCommandsMustBeEqualToNumberOfKeywords(string line)
            {
                //Assert.Fail();
                var matches = Regex.Matches(line, @"^\*[A-Za-z]", RegexOptions.Multiline);
                string msg = "";
                foreach (var item in matches)
                {
                    msg += item.ToString();
                }
                //Assert.Fail(msg);
                var number_of_keywords = matches.Count;
                lex.read_string(line);
                Assert.AreEqual(number_of_keywords, lex.commands.Count);
            }
        }
    }
}
