﻿using System;

namespace Kingdom.Collections
{
    /// <summary>
    /// These tests do not need to be complicated, we do not need to involve an actual
    /// Parent-Child relationship, even though that is at least one strong use case candidate.
    /// We can even base the tests upon <see cref="int"/>, just as long as the Item Type is
    /// <see cref="IEquatable{T}"/>.
    /// </summary>
    /// <inheritdoc />
    public abstract class TestFixtureBase : IDisposable
    {
        protected bool IsDisposed { get; private set; }

        /// <summary>
        /// Override this method in order to extend behavior <see cref="IDisposable"/>
        /// into a more test fixture context.
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void Dispose(bool disposing)
        {
        }

        public void Dispose()
        {
            Dispose(true);
            IsDisposed = true;
        }
    }
}
