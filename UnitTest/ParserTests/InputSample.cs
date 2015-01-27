using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UnitTest.ParserTests
{
    public class InputSample
    {
        public static IEnumerable<Input> NotAssemblySample
        {
            get
            {
                yield return new Input(@"*HEADING
セットなしケース
*NODE
1, 1
2,2
*ELEMENT, type=B31
1, 1, 2
");
                yield return new Input(@"*HEADING
要素集合のみ別途定義
*NODE
1, 1
2,2
*ELEMENT, type=B31
1, 1, 2
*ELSET, ELSET=BAR
1
");
                yield return new Input(@"*HEADING
節点と要素の定義時に集合を定義
*NODE, NSET=ALL_NODES
1, 1
2, 2.1, 3.4
*ELEMENT, ELSET=ALL_ELEMENTS
1, 1, 2
");
                yield return new Input(@"*HEADING
小文字
*node, nset=all_nodes
1, 1
2, 2.1, 3.4
*element, elset=global_elements
1, 1, 2
");
            }
        }

        public static IEnumerable<Input> WithAssemblySample
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
*ELEMENT, TYPE=B31, ELSET=baz
1, 1, 2
2, 2, 3
*end part
*assembly, name=Fuga
*instance, name=HogeHoge, part=hoge
*end instance
*end assembly
");
                yield return new Input(@"
*part, name=hoge
*NODE
1, 1.
2, 2.
3, 3.
*ELEMENT, TYPE=B31
1, 1, 2
2, 2, 3
*NSET, nset=bar
1, 2, 3
*ELSET, elset=elem
1, 2
*end part
*assembly, name=Fuga
*instance, name=HogeHoge, part=hoge
*end instance
*end assembly
");
                yield return new Input(@"
Nset generate
*part, name=hoge
*NODE
1, 1.
2, 2.
3, 3.
*ELEMENT, TYPE=B31
1, 1, 2
2, 2, 3
*NSET, nset=bar, generate
1, 3, 1
*ELSET, elset=elem, generate
1, 2, 1
*end part
*assembly, name=Fuga
*instance, name=HogeHoge, part=hoge
*end instance
*end assembly
");
                yield return new Input(@"
Nset generate
*part, name=hoge
*NODE
1, 1.
2, 2.
3, 3.
*ELEMENT, TYPE=B31
1, 1, 2
2, 2, 3
*NSET, nset=bar, generate
1, 3, 1
*ELSET, elset=elem, generate
1, 2, 1
*end part
*assembly, name=Fuga
*instance, name=Foo, part=hoge
*end instance
*NSET, NSET=ROOT
foo.1
*end assembly
");
                yield return new Input(@"
内部の節点集合を名前で参照
*part, name=hoge
*NODE
1, 1.
2, 2.
3, 3.
*ELEMENT, TYPE=B31
1, 1, 2
2, 2, 3
*NSET, nset=bar, generate
1, 3, 1
*ELSET, elset=elem, generate
1, 2, 1
*end part
*assembly, name=Fuga
*instance, name=Foo, part=hoge
*end instance
*NSET, NSET=BAR
foo.BAR
*end assembly
");
            }
        }
        

    }
}
