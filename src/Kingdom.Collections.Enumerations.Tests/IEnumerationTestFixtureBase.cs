using System;

namespace Kingdom.Collections
{
    using Xunit;

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IEnumerationTestFixtureBase<T> : IClassFixture<EnumerationCoverageReporter<T>>, IDisposable
        where T : Enumeration
    {
    }
}
