using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace System.Linq
{
    public static class LinqExtensions
    {
        public static async Task<IEnumerable<T>> WhenAll<T>(this IEnumerable<Task<T>> list)
        {
            return await Task.WhenAll(list);
        }
    }
}