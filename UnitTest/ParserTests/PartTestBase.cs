using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Abaqus;

namespace UnitTest.ParserTests
{

    public class PartTestBase : ParserTestBase
    {
        public static IEnumerable<Input> data
        {
            get
            {
                yield return new Input(@"*heading
*part, name=BAZ
*NODE, NSET=BAR
1, 1.
2, 2.
3, 3.
*ELEMENT, TYPE=B31
1, 1, 2
2, 2, 3
*end part
*assembly, name=Hoge
*instance, name=foo, part=baz
*end instance
*end assembly
");
                yield return new Input(@"*heading
*part, name=hoge
*NODE, Nset=BAR
1, 1.
2, 2.
3, 3.
*ELEMENT, TYPE=B31
1, 1, 2
2, 2, 3
*end part
*assembly, name=Fuga
*instance, name=HogeHoge, part=hoge
*end instance
*end assembly
");
            }
        }


        public static IEnumerable<Name> nset_names
        {
            get
            {
                yield return new Name("FOO.BAR");
                yield return new Name("HogeHoge.BAR");
            }
        }

         public uint[] ids = new uint[] { 1u, 2u, 3u };

    }
}
