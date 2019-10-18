using System;
using System.Reflection;

namespace Kingdom.Collections
{
    using Xunit;
    using Xunit.Abstractions;
    using Xunit.Sdk;
    using static Type;
    using static BindingFlags;

    /// <summary>
    /// Provides support for <see cref="OrdinalEnumeration{TDerived}"/> based unit tests.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <inheritdoc cref="DerivedEnumerationTestsBase{T}"/>
    public abstract class OrdinalEnumerationTestsBase<T> : DerivedEnumerationTestsBase<T>
        where T : OrdinalEnumeration<T>
    {
        /// <summary>
        /// Protected Constructor.
        /// </summary>
        /// <param name="outputHelper"></param>
        /// <param name="reporter"></param>
        /// <inheritdoc />
        protected OrdinalEnumerationTestsBase(ITestOutputHelper outputHelper
            , EnumerationCoverageReporter<T> reporter)
            : base(outputHelper, reporter)
        {
        }

        /// <summary>
        /// Reports whether the <typeparamref name="T"/> <see cref="Enumeration{TDerived}"/>
        /// type Has the Expected Constructors.
        /// </summary>
        /// <inheritdoc />
        public sealed override void Has_expected_Ctors() => NullInstance.HasExpectedOrdinalCtors();

#pragma warning disable xUnit1003 // Theory methods must have test data
        /// <summary>
        /// Verifies that the Enumerated <typeparamref name="T"/> Value is correct corresponding
        /// to the given parameters. Static data must also be provided at the time the tests are
        /// derived into the specific test case.
        /// </summary>
        /// <param name="ordinal"></param>
        /// <param name="name"></param>
        /// <param name="displayName"></param>
        // xunit: [Theory]
        public virtual void Verify_Enumeration_ordinal_value(int ordinal, string name, string displayName)
        {
            Assert.True(ordinal > 0);

            Assert.NotNull(name);
            Assert.NotEmpty(name);

            Assert.NotNull(displayName);
            Assert.NotEmpty(displayName);

            // TODO: fold in checking the GroupName
            var value = NullInstance.GetByOrdinal(ordinal);

            Assert.Same(value, NullInstance.GetByName(name));

            Assert.Same(value, NullInstance.GetByDisplayName(displayName));

            Reporter.Report(value.Name);
        }
#pragma warning restore xUnit1003 // Theory methods must have test data

    }

    /// <summary>
    /// Set of Internal extension methods used in support of the
    /// <see cref="OrdinalEnumerationTestsBase{T}"/>. These methods are necessary in order to
    /// isolate specific use cases for purposes of vetting the good, bad, and ugly test case
    /// paths in the tests tests.
    /// </summary>
    public static class OrdinalEnumerationTestExtensionMethods
    {
        /// <summary>
        /// Returns the <typeparamref name="T"/> Value by its <paramref name="ordinal"/>.
        /// <paramref name="value"/> is provided to connect the caller with the
        /// <see cref="Enumeration{TDerived}"/> <typeparamref name="T"/>. Nothing more,
        /// nothing less.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="ordinal"></param>
        /// <param name="verify"></param>
        /// <returns></returns>
        public static T GetByOrdinal<T>(this T value, int ordinal, Action<T> verify = null)
            where T : OrdinalEnumeration<T>
        {
            var result = OrdinalEnumeration<T>.FromOrdinal(ordinal);
            Assert.NotNull(result);
            Assert.Equal(ordinal, result.Ordinal);
            return result.VerifyEnumeration(verify);
        }

        /// <summary>
        /// Connect the verification with the <see cref="Enumeration"/> <paramref name="value"/>
        /// only. We do not require the connection with the test class itself. In fact, we want it
        /// to be separate, so we can do some independent verification of different good, bad, or
        /// ugly use cases. Returns whether the type <typeparamref name="T"/> Has the Expected
        /// Constructors. That is, there is One <see cref="NonPublic"/>, <see cref="Instance"/>
        /// Constructor accepting a single <see cref="int"/> parameter. The Constructor must also be
        /// <see cref="MethodBase.IsPrivate"/>.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <exception cref="TrueException">Thrown when the Constructor is not
        /// <see cref="MethodBase.IsPrivate"/>.</exception>
        /// <exception cref="NotNullException">Thrown when the Constructor cannot be
        /// found.</exception>
        /// <see cref="ConstructorInfo"/>
        /// <see cref="MethodBase.IsPrivate"/>
        /// <see cref="NonPublic"/>
        /// <see cref="Instance"/>
        /// <see cref="int"/>
        internal static void HasExpectedOrdinalCtors<T>(this T value)
            where T : OrdinalEnumeration<T>
        {
            var ctor = typeof(T).GetConstructor(NonPublic | Instance
                , DefaultBinder, new[] {typeof(int)}, null);
            Assert.NotNull(ctor);
            Assert.True(ctor.IsPrivate);
        }
    }
}
