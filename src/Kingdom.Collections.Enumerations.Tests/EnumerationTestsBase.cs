using System;

namespace Kingdom.Collections
{
    using Xunit;
    using Xunit.Abstractions;
    using Xunit.Sdk;

    /// <summary>
    /// Provides <see cref="Enumeration"/> based test support.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <inheritdoc />
    public abstract class EnumerationTestsBase<T> : IDisposable
        where T : Enumeration
    {
        /// <summary>
        /// Provides a single NullInstance for use throughout the framework.
        /// </summary>
        protected static readonly T NullInstance = null;

        /// <summary>
        /// Gets the <see cref="ITestOutputHelper"/> instance.
        /// </summary>
        protected ITestOutputHelper OutputHelper { get; }

        /// <summary>
        /// Protected Constructor.
        /// </summary>
        /// <param name="outputHelper"></param>
        protected EnumerationTestsBase(ITestOutputHelper outputHelper)
        {
            OutputHelper = outputHelper;
        }

        /// <summary>
        /// By definition, Enumerations are not to have Public Constructors.
        /// </summary>
        [Fact]
        public abstract void Shall_not_have_any_Public_Ctors();

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

    /// <summary>
    /// Provides a set of <see cref="Enumeration"/> based extension methods.
    /// </summary>
    public static class EnumerationExtensionMethods
    {
        /// <summary>
        /// Verifies the <paramref name="value"/> using the <paramref name="verify"/> action.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <param name="verify"></param>
        /// <returns></returns>
        /// <exception cref="NotNullException">Thrown when the <paramref name="value"/> is
        /// Null.</exception>
        public static T VerifyEnumeration<T>(this T value, Action<T> verify = null)
            where T : Enumeration
        {
            Assert.NotNull(value);
            /* We do not expect the Action to be Null at this moment,
             but do this for safety nonetheless. */
            verify?.Invoke(value);
            return value;
        }
    }
}
