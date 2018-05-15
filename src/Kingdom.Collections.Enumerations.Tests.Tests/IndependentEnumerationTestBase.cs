using System;

namespace Kingdom.Collections
{
    using Xunit;

    public abstract class IndependentEnumerationTestBase<T> : IDisposable
        where T : Enumeration<T>
    {
        protected static readonly T NullInstance = null;

        [Fact]
        public void Values_collection_is_always_returned_regardless()
        {
            Assert.NotNull(Enumeration<T>.Values);
        }

        /// <summary>
        /// Gets whether the object IsDisposed.
        /// </summary>
        protected bool IsDisposed { get; private set; }

        /// <summary>
        /// Disposes the object.
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void Dispose(bool disposing)
        {
        }

        /// <summary>
        /// Disposes the object.
        /// </summary>
        /// <inheritdoc />
        public void Dispose()
        {
            Dispose(true);
            IsDisposed = true;
        }
    }
}
