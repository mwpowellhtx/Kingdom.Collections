using System;
using System.Linq;
using System.Reflection;

namespace Kingdom.Collections
{
    using Xunit;
    using Xunit.Abstractions;
    using static BindingFlags;

    /// <summary>
    /// Provides Base related <see cref="BaseEnumeration{T}"/> unit tests.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class BaseEnumerationTestsBase<T>
        where T : BaseEnumeration<T>
    {
        /// <summary>
        /// Provides a single NullInstance for use throughout the framework.
        /// </summary>
        protected static readonly T NullInstance = null;

        /// <summary>
        /// Gets the OutputHelper.
        /// </summary>
        protected ITestOutputHelper OutputHelper { get; }

        /// <summary>
        /// Gets the Reporter.
        /// </summary>
        protected BaseEnumerationCoverageReporter<T> Reporter { get; }

        /// <summary>
        /// Protected Constructor.
        /// </summary>
        /// <param name="outputHelper"></param>
        /// <param name="reporter"></param>
        /// <inheritdoc />
        protected BaseEnumerationTestsBase(ITestOutputHelper outputHelper
            , BaseEnumerationCoverageReporter<T> reporter)
        {
            OutputHelper = outputHelper;
            Reporter = reporter;
        }

        /// <summary>
        /// Verifies whether <see cref="BaseEnumeration{T}"/> Has the Expected Constructors.
        /// </summary>
        /// <see cref="EnumerationTestsBase{T}.NullInstance"/>
        /// <see cref="BaseEnumerationTestExtensionMethods.HasExpectedCtors{T}"/>
        [Fact]
        public void Has_expected_Ctors()
        {
            NullInstance.HasExpectedCtors();
            Reporter.Report(BaseEnumeration<T>.Values.Select(x => x.Name).ToArray());
        }


#pragma warning disable xUnit1003 // Theory methods must have test data
        /// <summary>
        /// Verifies that the Enumerated <typeparamref name="T"/> Value is correct corresponding
        /// to the given parameters. Static data must also be provided at the time the tests are
        /// derived into the specific test case.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="displayName"></param>
        [Theory]
        public virtual void Verify_Base_Enumeration_Names(string name, string displayName)
        {
            var value = NullInstance.GetBaseByName(name.AssertNotNull().AssertNotEmpty());
            var other = NullInstance.GetBaseByDisplayName(displayName.AssertNotNull().AssertNotEmpty());
            other.AssertSame(value);
            Reporter.Report(value.Name);
        }
#pragma warning restore xUnit1003 // Theory methods must have test data

    }

    /// <summary>
    /// Set of Internal extension methods used in support of the
    /// <see cref="BaseEnumerationTestsBase{T}"/>. These methods are necessary in order to
    /// isolate specific use cases for purposes of vetting the good, bad, and ugly test case
    /// paths in the tests tests.
    /// </summary>
    public static class BaseEnumerationTestExtensionMethods
    {
        /// <summary>
        /// Returns the <see cref="BaseEnumeration{T}"/> <typeparamref name="T"/> Value by its
        /// <paramref name="name"/>. <paramref name="_"/> is provided to connect the caller
        /// with the <see cref="BaseEnumeration{T}"/> <typeparamref name="T"/>. Nothing more,
        /// nothing less.
        /// </summary>
        /// <param name="_"></param>
        /// <param name="name"></param>
        /// <param name="comparison"></param>
        /// <returns></returns>
        /// <see cref="BaseEnumeration{T}.FromName"/>
        /// <see cref="BaseEnumeration{T}.FromDisplayName"/>
        public static T GetBaseByName<T>(this T _, string name, StringComparison? comparison = null)
            where T : BaseEnumeration<T>
        {
            var result = BaseEnumeration<T>.FromName(name, comparison).AssertNotNull();
            return result.AssertEqual(name, x => x.Name);
        }

        /// <summary>
        /// Returns the <see cref="BaseEnumeration{T}"/> instance corresponding
        /// to the <paramref name="displayName"/>. Optionally receives a
        /// <paramref name="comparison"/> parameter.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="_"></param>
        /// <param name="displayName"></param>
        /// <param name="comparison"></param>
        /// <returns></returns>
        public static T GetBaseByDisplayName<T>(this T _, string displayName, StringComparison? comparison = null)
            where T : BaseEnumeration<T>
        {
            var result = BaseEnumeration<T>.FromDisplayName(displayName, comparison);
            result.AssertNotNull();
            return result.AssertEqual(displayName, x => x.DisplayName);
        }

        /// <summary>
        /// Connect the verification with the <see cref="BaseEnumeration{T}"/>
        /// <paramref name="value"/> only. We do not require the connection with the test class
        /// itself. In fact, we want it to be separate, so we can do some independent verification
        /// of different good, bad, or ugly use cases. Returns whether the type
        /// <typeparamref name="T"/> Has the Expected Bitwise Constructors. That is, there is
        /// One <see cref="NonPublic"/>, <see cref="Instance"/> Constructor accepting a single
        /// <see cref="int"/> parameter. The Constructor must also be
        /// <see cref="MethodBase.IsPrivate"/>.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <see cref="ConstructorInfo"/>
        /// <see cref="MethodBase.IsPrivate"/>
        /// <see cref="NonPublic"/>
        /// <see cref="Instance"/>
        /// <see cref="T:byte[]"/>
        internal static void HasExpectedCtors<T>(this T value)
            where T : BaseEnumeration<T>
        {
            var type = typeof(T);

            var ctors = type.GetConstructors(NonPublic | Instance).AssertNotNull().AssertNotEmpty();

            ctors.OrderBy(ci => ci.GetParameters())
                .AssertCollection(
                    ci => ci.AssertTrue(x => x.IsPrivate).GetParameters().AssertNotNull().AssertEmpty()
                );
        }
    }
}
