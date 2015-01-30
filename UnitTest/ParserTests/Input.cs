using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace UnitTest.ParserTests
{
    public class Input: Data
    {
        public Input(string str, bool is_valid = true) :base(str, is_valid)
        {
        }
        public bool isValid() { return valid; }
    }


    public class InputTest
    {
        [Datapoints] string [] codes = new string[] {"hoge", "foo", "bar", ""};


        [Theory]
        public void ValidTest(string code, bool validate)
        {
            var inp = new Input(code, validate);
            //Assert.Fail("ValidTest");
            Assert.AreEqual(validate, inp.valid);
        }

        [Theory]
        public void CodeTest(string code, bool validate)
        {
            var inp = new Input(code, validate);
            //Assert.Fail("ValidTest");
            Assert.AreEqual(code, inp.Value);
        }

    }

}
