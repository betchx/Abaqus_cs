using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UnitTest.ParserTests
{
    public class Name : Data
    {
        public Name(string str, bool is_valid = true):base(str, is_valid)
        {
        }
    }
}
