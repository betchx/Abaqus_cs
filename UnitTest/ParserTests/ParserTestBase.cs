using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Abaqus;
using NUnit.Framework;

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

    }
}
