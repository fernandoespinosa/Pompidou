using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Pompidou
{
    class OperableEnumerable<T> : IEnumerable<T>
    {
        private readonly IEnumerable<T> _underlying;

        public static IEnumerable<T> operator +(OperableEnumerable<T> e1, IEnumerable<T> e2)
        {
            return e1.Concat(e2);
        }

        public IEnumerator<T> GetEnumerator()
        {
            return _underlying.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _underlying.GetEnumerator();
        }

        public OperableEnumerable(IEnumerable<T> underlying)
        {
            _underlying = underlying;
        }
    }
}