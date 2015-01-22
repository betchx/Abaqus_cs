using System;
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
            var parser = new Abaqus.Parser();
            Abaqus.Model model = parser.parse_file("Simple.inp");
            foreach (var k in model.nsets.Keys)
            {
                System.Console.Out.WriteLine(k);
            }
        }

    }
}
