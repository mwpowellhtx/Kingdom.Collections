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
    /// Provides Bitwise related <see cref="Enumeration{TDerived}"/> unit tests.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <inheritdoc />
    public abstract class BitwiseEnumerationTestsBase<T> : DerivedEnumerationTestsBase<T>
        where T : Enumeration<T>
    {
        /// <summary>
        /// Protected Constructor.
        /// </summary>
        /// <param name="outputHelper"></param>
        /// <param name="reporter"></param>
        /// <inheritdoc />
        protected BitwiseEnumerationTestsBase(ITestOutputHelper outputHelper
            , EnumerationCoverageReporter<T> reporter)
            : base(outputHelper, reporter)
        {
        }

        /// <summary>
        /// Verifies whether <see cref="Enumeration{TDerived}"/> Has the Expected Constructors.
        /// </summary>
        /// <see cref="EnumerationTestsBase{T}.NullInstance"/>
        /// <see cref="BitwiseEnumerationTestExtensionMethods.HasExpectedBitwiseCtors{T}"/>
        /// <inheritdoc />
        public sealed override void Has_expected_Ctors() => NullInstance.HasExpectedBitwiseCtors();

#pragma warning disable xUnit1003 // Theory methods must have test data
        /// <summary>
        /// Verifies that the Enumerated <typeparamref name="T"/> Value is correct corresponding
        /// to the given parameters. Static data must also be provided at the time the tests are
        /// derived into the specific test case.
        /// </summary>
        /// <param name="bytes"></param>
        /// <param name="name"></param>
        /// <param name="displayName"></param>
        // xunit: [Theory]
        public virtual void Verify_Bitwise_Enumeration_bits(byte[] bytes, string name, string displayName)
        {
            Assert.NotNull(bytes);
            Assert.NotEmpty(bytes);

            // There must be at least one Non Zero Byte.
            Assert.Contains(bytes, x => x > 0);

            Assert.NotNull(name);
            Assert.NotEmpty(name);

            Assert.NotNull(displayName);
            Assert.NotEmpty(displayName);

            // TODO: fold in checking the GroupName
            var value = NullInstance.GetByBytes(bytes);

            Assert.Same(value, NullInstance.GetByName(name));

            Assert.Same(value, NullInstance.GetByDisplayName(displayName));

            Reporter.Report(value.Name);
        }
#pragma warning restore xUnit1003 // Theory methods must have test data

    }

    /// <summary>
    /// Set of Internal extension methods used in support of the
    /// <see cref="BitwiseEnumerationTestsBase{T}"/>. These methods are necessary in order to
    /// isolate specific use cases for purposes of vetting the good, bad, and ugly test case
    /// paths in the tests tests.
    /// </summary>
    public static class BitwiseEnumerationTestExtensionMethods
    {
        /// <summary>
        /// Returns the <see cref="Enumeration{TDerived}"/> <typeparamref name="T"/> Value by its
        /// <paramref name="bytes"/>. <paramref name="value"/> is provided to connect the caller
        /// with the <see cref="Enumeration{TDerived}"/> <typeparamref name="T"/>. Nothing more,
        /// nothing less.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="bytes"></param>
        /// <returns></returns>
        /// <exception cref="NotNullException">Thrown when the value is Null.</exception>
        /// <exception cref="EqualException">Thrown when the bits are not equal.</exception>
        /// <see cref="ImmutableBitArray"/>
        /// <see cref="Enumeration{TDerived}.FromBitArray"/>
        public static T GetByBytes<T>(this T value, byte[] bytes)
            where T : Enumeration<T>
        {
            var bits = new ImmutableBitArray(bytes);
            var result = Enumeration<T>.FromBitArray(bits);
            Assert.NotNull(result);
            Assert.True(result.Bits.Equals(bits)); // nunit
            // xunit: Assert.Equal(bits, result.Bits);
            return result;
        }

        /// <summary>
        /// Connect the verification with the <see cref="Enumeration{TDerived}"/>
        /// <paramref name="value"/> only. We do not require the connection with the test class
        /// itself. In fact, we want it to be separate, so we can do some independent verification
        /// of different good, bad, or ugly use cases. Returns whether the type
        /// <typeparamref name="T"/> Has the Expected Bitwise Constructors. That is, there is
        /// One <see cref="NonPublic"/>, <see cref="Instance"/> Constructor accepting a single
        /// <see cref="int"/> paramter. The Constructor must also be
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
        /// <see cref="T:byte[]"/>
        internal static void HasExpectedBitwiseCtors<T>(this T value)
            where T : Enumeration<T>
        {
            var ctor = typeof(T).GetConstructor(NonPublic | Instance, DefaultBinder, new[] {typeof(byte[])}, null);
            Assert.NotNull(ctor);
            Assert.True(ctor.IsPrivate);
        }
    }
}
