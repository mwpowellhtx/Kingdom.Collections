using System;

namespace Kingdom.Collections
{
    internal abstract class Disposable : IDisposable
    {
        protected Disposable()
        {
        }

        private bool _disposed;

        public bool IsDisposed
        {
            get { return _disposed; }
        }

        protected virtual void Dispose(bool disposing)
        {
            _disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
        }
    }
}
