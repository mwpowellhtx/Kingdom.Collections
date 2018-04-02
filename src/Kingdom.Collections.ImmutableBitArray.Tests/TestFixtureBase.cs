using System;

namespace Kingdom.Collections
{
    using NUnit.Framework;

    public abstract class TestFixtureBase : IDisposable
    {
        [TestFixtureSetUp]
        public virtual void TestFixtureSetUp()
        {
        }

        [TestFixtureTearDown]
        public virtual void TestFixtureTearDown()
        {
        }

        [SetUp]
        public virtual void SetUp()
        {
        }

        [TearDown]
        public virtual void TearDown()
        {
        }

        public bool IsDisposed { get; private set; }

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
