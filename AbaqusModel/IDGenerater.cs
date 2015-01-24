using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Abaqus
{
    internal class IDGenerater : IEnumerable<uint>
    {
        uint start, last, step;
        public IDGenerater(string line)
        {
            var arr = line.Split(',').Select(s => uint.Parse(s)).ToArray();
            start = arr[0];
            last = arr[1];
            step = arr[2];
        }
        public IEnumerator<uint> GetEnumerator()
        {
            return new GenerateEnumerator(start, last, step);
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return new GenerateEnumerator(start, last, step);
        }

        // Enumeratorクラスを作成してしまったので，そちらを使用することにした．
        //public System.Collections.Generic.IEnumerator<uint> GetEnumerator()
        //{
        //    for (uint i = start; i <= last; i+=step)
        //    {
        //        yield return i;
        //    }
        //}


    }
}
