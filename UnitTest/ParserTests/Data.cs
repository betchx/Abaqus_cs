using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UnitTest.ParserTests
{
    public class Data
    {
        public string Value { get; private set; }
        public Data(string s, bool is_valid = true)
        {
            Value = s;
            valid = is_valid;
        }
        public static implicit operator string(Data i) { return i.Value; }
        public static implicit operator bool(Data i) { return i.valid; }

        public bool valid { get; set; }
    }
}
