using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Abaqus;
using NUnit.Framework;
using System.Text.RegularExpressions;

namespace UnitTest.ParserTests
{
    [TestFixture]
    public class ParserTestBase
    {
        internal Parser parser;
        internal Lexer lexer;

        [SetUp]
        public void Setup()
        {
            parser = new Parser();
            lexer = new Lexer();
        }

        public static Regex parameter_key(string key)
        {
            return new Regex(key+@"");
//            return new Regex(@"^\*\a[^,]* *," + key + @" *= *([^,\n\r]+)");
        }
        public static Regex parameter_key_value(string key)
        {
            return new Regex(@"^\*" + key + @" *= *([^,\n\r]+)");
        }
    }

    [TestFixture]
    public class KeyTest
    {
        public struct ParamTestData
        {
            public string data;
            public string key;
            public string value;
            public ParamTestData (string data, string key, string value)
            {
                this.data = data;
                this.key = key;
                this.value = value;
            }
        }

        [Datapoints]
        public IEnumerable<ParamTestData> keywordlines
        {
            get
            {
                yield return new ParamTestData("*keyword, param", "param",null);
                yield return new ParamTestData("*keyword, param=value", "param","value");
                yield return new ParamTestData("*NODE, NSET=ABC","NSET","ABC");
                yield return new ParamTestData("*ELEMENT, ELSET=RAIL, TYPE=B31", "ELSET", "RAIL");
                yield return new ParamTestData("*ELEMENT, ELSET=RAIL, TYPE=B31", "TYPE", "B31");
                yield return new ParamTestData("*ELEMENT, ELSET=RAIL, TYPE=B31\n1,1,2\n", "TYPE", "B31");
            }
        }

        public void CheckIsNotNullOrEmpty(
            [Values("", "a", null)] string data
            )
        {
            Assume.That(data, Is.Not.Null.Or.Empty);
            Assert.That(data, Is.Not.Null);
            Assert.That(data, Is.Not.Empty);
        }

        [Theory]
        public void parameter_keyはパラメータ名にマッチする(ParamTestData args)
        {
            // Assumptions
            Assume.That(args, Is.Not.Null);
            Assume.That(args.data, Is.Not.Null);
            Assume.That(args.key, Is.Not.Null);

            // check that assumption is valid
            //Assert.Fail(args.data);
            // Work

            var regex = ParserTestBase.parameter_key(args.key);
            var res = regex.Matches(args.data);

            // Assertions
            Assert.That(res.Count, Is.GreaterThan(0));
        }
    }
}
