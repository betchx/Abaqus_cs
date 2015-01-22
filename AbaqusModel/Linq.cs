using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Abaqus
{
    public static class Linq
    {
        public static void ForEach<T>(this IEnumerable<T> source, Action<T> action)
        {
            foreach (var item in source)
                action(item);
        }
    }
}
