using System;
using System.Collections.Generic;
using System.Linq;

namespace Kingdom.Collections
{
    public abstract class ValuesFixtureBase<T>
    {
        internal IEnumerable<T> Values { get; set; }

        protected abstract string ToString(T value);

        private readonly Lazy<string> _lazyString;

        public override string ToString()
        {
            return _lazyString.Value;
        }

        internal int Size { get; private set; }

        internal int ExpectedLength
        {
            get { return Values.Count()*Size; }
        }

        protected ValuesFixtureBase(Func<int> getSize)
        {
            Size = getSize();
            _lazyString = new Lazy<string>(() => string.Join(string.Empty, Values.Select(ToString)));
        }
    }
}
