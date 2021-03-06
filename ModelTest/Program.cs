﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Abaqus;

namespace ModelTest
{
    internal static class Linq
    {
        public static void ForEach<T>(this IEnumerable<T> source, Action<T> action)
        {
            foreach (var item in source)
                action(item);
        }
    }

    class Program
    {


        static void Main(string[] args)
        {
#if false
            NUnit.Gui.AppEntry.Main(new string[]{
                System.Windows.Forms.Application.ExecutablePath, "/run"});
#endif

            var test = new UnitTest.ParserTests.ParserTest();
            //test.setup();
            //test.ParseElementTest();
            //test.teardown();

            var parser = new Abaqus.Parser();
            //var model = parser.parse_string(UnitTest.ParserTests.ParserTest.inp1);
            var model = parser.parse_string(UnitTest.LexTest.input);

#if false
            Abaqus.Model model = parser.parse_file("Simple.inp");
            foreach (var k in model.nsets.Keys)
            {
                System.Console.Out.WriteLine(k);
            }
#endif
        }

    }
}
