using System.Collections.Generic;

namespace Pompidou
{
    static class Extensions
    {
        public static OperableEnumerable<T> Operable<T>(this IEnumerable<T> enumerable)
        {
            return new OperableEnumerable<T>(enumerable);
        }

        public static string Join<T>(this IEnumerable<T> enumerable, string separator)
        {
            return string.Join(separator, enumerable);
        }
    }
}