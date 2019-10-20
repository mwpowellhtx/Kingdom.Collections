using System;

namespace Kingdom.Collections
{
    using Xunit.Abstractions;

    /// <inheritdoc cref="KeyedEnumerationTestFixtureBase{TKey,T}" />
    /// <see cref="OrdinalEnumeration{TKey,T}"/>
    public abstract class OrdinalEnumerationTestFixtureBase<TKey, T> : KeyedEnumerationTestFixtureBase<TKey, T>
        where T : OrdinalEnumeration<TKey, T>
        where TKey : struct, IComparable<TKey>, IEquatable<TKey>
    {
        /// <summary>
        /// Protected Constructor.
        /// </summary>
        /// <param name="outputHelper"></param>
        /// <param name="reporter"></param>
        /// <inheritdoc />
        protected OrdinalEnumerationTestFixtureBase(ITestOutputHelper outputHelper, IEnumerationCoverageReporter<T> reporter)
            : base(outputHelper, reporter)
        {
        }
    }
}
