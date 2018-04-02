using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Kingdom.Collections
{
    using NUnit.Framework;
    using static String;
    using static BindingFlags;

    /// <summary>
    /// Represents <see cref="Enumeration{T}"/> derived tests support.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <inheritdoc cref="EnumerationTestsBase{T}" />
    public abstract class DerivedEnumerationTestsBase<T>
        : EnumerationTestsBase<T>
            , IClassFixture<EnumerationCoverageReporter<T>>
        where T : Enumeration<T>
    {
        /// <summary>
        /// Gets the Reporter.
        /// </summary>
        protected EnumerationCoverageReporter<T> Reporter { get; }

        /// <summary>
        /// Protected Constructor.
        /// </summary>
        /// <param name="outputHelper"></param>
        /// <param name="reporter"></param>
        /// <inheritdoc />
        protected DerivedEnumerationTestsBase(ITestOutputHelper outputHelper, EnumerationCoverageReporter<T> reporter)
            : base(outputHelper)
        {
            // We want exactly One Reporter across the entire range of tests.
            Reporter = reporter;
        }

        /// <summary>
        /// By definition, Enumerations are not to have Public Constructors.
        /// </summary>
        /// <see cref="EnumerationTestsBase{T}.NullInstance"/>
        /// <see cref="DerivedEnumerationTestExtensionMethods.ShallNotHaveAnyPublicCtors{T}"/>
        /// <inheritdoc />
        public override void Shall_not_have_any_Public_Ctors() => NullInstance.ShallNotHaveAnyPublicCtors(Reporter);

        /// <summary>
        /// Reports whether the Enumeration <typeparamref name="T"/> Has Expected Constructors.
        /// </summary>
        [Test]
        public abstract void Has_expected_Ctors();

        /// <summary>
        /// Reports whether the <typeparamref name="T"/> <see cref="Enumeration{TDerived}"/>
        /// Shall Have at least One Value.
        /// </summary>
        /// <see cref="EnumerationTestsBase{T}.NullInstance"/>
        /// <see cref="DerivedEnumerationTestExtensionMethods.ShallHaveAtLeastOneValue{T}"/>
        [Test]
        public void Shall_have_at_least_One_Value() => NullInstance.ShallHaveAtLeastOneValue(Reporter);

        /// <summary>
        /// Verifies that the <see cref="Enumeration{TDerived}.Values"/>
        /// <see cref="Enumeration.Bits"/> Length are all Consistent.
        /// </summary>
        /// <see cref="EnumerationTestsBase{T}.NullInstance"/>
        /// <see cref="DerivedEnumerationTestExtensionMethods.ShallAllHaveConsistentBitLengths{T}"/>
        /// <see cref="DerivedEnumerationTestsBase{T}.Reporter"/>
        [Test]
        public void Values_shall_all_have_consistent_Bits_Length()
            => NullInstance.ShallAllHaveConsistentBitLengths(false, Reporter);

        /// <summary>
        /// Gaps in the Bits Assignments are acceptable, however, unassigned Bits are not
        /// acceptable. For now, the assumption applies for both bitwise
        /// <see cref="Enumeration{TDerived}"/> as well as
        /// <see cref="OrdinalEnumeration{TDerived}"/>.
        /// </summary>
        /// <see cref="DerivedEnumerationTestExtensionMethods.ValueBitsShallBeUniquelyAssigned{T}"/>
        /// <see cref="DerivedEnumerationTestsBase{T}.Reporter"/>
        /// <see cref="EnumerationTestsBase{T}.OutputHelper"/>
        [Test]
        public void Value_Bits_shall_be_uniquely_assigned()
            => NullInstance.ValueBitsShallBeUniquelyAssigned(Reporter, OutputHelper);
    }

    /// <summary>
    /// Provides a set of extension methods supporting
    /// <see cref="DerivedEnumerationTestsBase{T}"/> tests.
    /// </summary>
    public static class DerivedEnumerationTestExtensionMethods
    {
        /// <summary>
        /// Returns the <typeparamref name="T"/> Value by its <paramref name="name"/>.
        /// <paramref name="value"/> is provided to connect the caller with the
        /// <see cref="Enumeration{TDerived}"/> <typeparamref name="T"/>. Nothing more,
        /// nothing less.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="name"></param>
        /// <param name="verify"></param>
        /// <returns></returns>
        public static T GetByName<T>(this T value, string name, Action<T> verify = null)
            where T : Enumeration<T>
        {
            var result = Enumeration<T>.FromName(name);
            Assert.NotNull(result);
            Assert.That(result.Name, Is.EqualTo(name)); // nunit
            // xunit: Assert.Equals(name, result.Name);
            return result.Verify(verify);
        }

        /// <summary>
        /// Returns the <typeparamref name="T"/> Value by its <paramref name="displayName"/>.
        /// <paramref name="value"/> is provided to connect the caller with the
        /// <see cref="Enumeration{TDerived}"/> <typeparamref name="T"/>. Nothing more,
        /// nothing less.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="displayName"></param>
        /// <param name="verify"></param>
        /// <returns></returns>
        public static T GetByDisplayName<T>(this T value, string displayName, Action<T> verify = null)
            where T : Enumeration<T>
        {
            var result = Enumeration<T>.FromDisplayName(displayName);
            Assert.NotNull(result);
            Assert.That(result.DisplayName, Is.EqualTo(displayName)); // nunit
            // xunit: Assert.Equal(displayName, result.DisplayName);
            return result.Verify(verify);
        }

        /// <summary>
        /// Connect the verification with the <see cref="Enumeration{TDerived}"/>
        /// <paramref name="value"/> only. We do not require the connection with the test class
        /// itself. In fact, we want it to be separate, so we can do some independent verification
        /// of different good, bad, or ugly use cases. We expect there to be no
        /// <see cref="Public"/>, <see cref="Instance"/> Constructors.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <param name="reporter"></param>
        /// <exception cref="EmptyException">Occurs when there are not Constructors matching the pattern.</exception>
        /// <see cref="Public"/>
        /// <see cref="Instance"/>
        /// <see cref="Type.GetConstructors(BindingFlags)"/>
        internal static void ShallNotHaveAnyPublicCtors<T>(this T value,
            IEnumerationCoverageReporter<T> reporter = null)
            where T : Enumeration<T>
        {
            if (reporter != null)
            {
                reporter.Enabled = false;
            }

            // We do not care about the Value apart from identifying the underlying Enumeration Type.
            CollectionAssert.IsEmpty(typeof(T).GetConstructors(Public | Instance)); // nunit
            // xunit: Assert.Empty(typeof(T).GetConstructors(Public | Instance));
        }

        /// <summary>
        /// Verifies that the type <typeparamref name="T"/> Shall Have At Least One Value. We do
        /// not care about the <paramref name="value"/> itself apart from its connection with the
        /// generic type <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <param name="reporter"></param>
        /// <returns></returns>
        /// <exception cref="NotNullException">Thrown when the <see cref="Enumeration{TDerived}.Values"/> are Null.</exception>
        /// <exception cref="TrueException">Thrown when there are Not Any Enumerated Values.</exception>
        /// <see cref="Enumeration{TDerived}.Values"/>
        /// <see cref="Enumerable.Any{TSource}(IEnumerable{TSource})"/>
        internal static T ShallHaveAtLeastOneValue<T>(this T value, IEnumerationCoverageReporter<T> reporter = null)
            where T : Enumeration<T>
        {
            if (reporter != null)
            {
                reporter.Enabled = false;
            }

            /* Do not Report any Names at this level; this is intentional. Rather, do leave that
             level of reporting for the actual unit test coverage. This ensures that we enforce
             appropriate Enumeration Test Coverage at appropriate levels. */
            var values = Enumeration<T>.Values.ToArray();
            Assert.NotNull(values);
            Assert.True(values.Any());
            return value;
        }

        /// <summary>
        /// <typeparamref name="T"/> <see cref="Enumeration{TDerived}"/> values Shall All have
        /// Consistent <see cref="Enumeration.Bits"/> lengths. Rules out false positives in the
        /// form of empty <see cref="Enumeration{TDerived}.Values"/>.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <param name="ignoreNulls"></param>
        /// <param name="reporter"></param>
        /// <exception cref="NotNullException">Should never occur, but we verify that there is
        /// at least a <see cref="Enumeration{TDerived}.Values"/> collection.</exception>
        /// <exception cref="NotEmptyException">Asserts that there is at least One
        /// <see cref="Enumeration{TDerived}.Values"/> item.</exception>
        /// <exception cref="TrueException">Asserts that there are
        /// <see cref="Enumerable.Any{TSource}(IEnumerable{TSource})"/>
        /// <see cref="Enumeration.Bits"/>.</exception>
        /// <exception cref="SingleException">Occurs when the <see cref="Enumeration.Bits"/>
        /// lengths are inconsistent.</exception>
        internal static void ShallAllHaveConsistentBitLengths<T>(this T value
            , bool ignoreNulls = true, IEnumerationCoverageReporter<T> reporter = null)
            where T : Enumeration<T>
        {
            // Which assumes that we actually have values...
            var values = Enumeration<T>.GetValues(ignoreNulls).ToArray();

            Assert.NotNull(values);
            CollectionAssert.IsNotEmpty(values); // nunit
            // xunit: Assert.NotEmpty(values);

            // We do not care that it was One Byte or One Hundred, only that it was Consistent.
            Assert.That(values.Select(x =>
            {
                Assert.NotNull(x);
                /* There must be some Bits for this to work...
                 Yes, while we could Assert NotEmpty here, I am trying to keep the thrown
                 Exceptions as distinct as possible, not least of which for unit test
                 purposes. */
                Assert.True(x.Bits.Any());
                var result = x.Bits.Length;
                reporter?.Report(x.Name);
                return result;
            }).Distinct().ToArray(), Has.Length.EqualTo(1)); // nunit

            // xunit: Assert.Single(values.Select(x =>
            //{
            //    Assert.NotNull(x);
            //    /* There must be some Bits for this to work...
            //     Yes, while we could Assert NotEmpty here, I am trying to keep the thrown
            //     Exceptions as distinct as possible, not least of which for unit test
            //     purposes. */
            //    Assert.True(x.Bits.Any());
            //    var result = x.Bits.Length;
            //    reporter?.Report(x.Name);
            //    return result;
            //}).Distinct());
        }

        /// <summary>
        /// <paramref name="value"/> is used to connect the caller with the underlying
        /// <see cref="Enumeration{TDerived}"/> type only. Additionally, we are making the
        /// assumption that <see cref="Enumeration{TDerived}.Values"/> are unique not only in
        /// <see cref="Enumeration.Name"/>, but also in <see cref="Enumeration.Bits"/> value.
        /// In truth, this could go either way, but for now I will expect that there must be
        /// uniqueness throughout.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="reporter"></param>
        /// <param name="outputHelper"></param>
        /// <typeparam name="T"></typeparam>
        /// <see cref="!:http://en.wikipedia.org/wiki/Enumeration"/>
        /// <exception cref="AllException">Occurs when not All the Values Bits are assigned.</exception>
        /// <exception cref="TrueException">Occurs when a Value Bits are not assigned.</exception>
        /// <exception cref="EqualException">Thrown when the Values Length is different from the
        /// <see cref="Enumerable.Distinct{TSource}(IEnumerable{TSource})"/> Values. This entirely depends upon
        /// <see cref="Enumeration"/> providing the required behavior for Distinct to work properly.</exception>
        public static void ValueBitsShallBeUniquelyAssigned<T>(this T value
            , IEnumerationCoverageReporter<T> reporter = null
            , ITestOutputHelper outputHelper = null)
            where T : Enumeration<T>
        {
            var values = Enumeration<T>.Values.Select(x =>
            {
                var result = x;
                reporter?.Report(x.Name);
                return result;
            }).ToArray();

            // The Bits must All be assigned to Something.
           Assert.True(values.All(x =>
           {
               var any = x.Bits.Any();
               Assert.True(any);
               return any;
           })); // nunit
            // xunit: Assert.All(values, x => Assert.True(x.Bits.Any()));

            /* This step is key; Bits must all be Uniquely Assigned, that is, Groups of One.
             It does not matter if those Bits originated as a pure Bitwise Enumeration, or
             the Ordinal variety. */
            var grouped = values.GroupBy(x => x.Bits.ToByteString()).ToArray();

            try
            {
                Assert.That(grouped, Has.Length.EqualTo(values.Length)); // nunit
                // xunit: Assert.Equal(values.Length, grouped.Length);
            }
            catch (Exception)
            {
                Func<string> listDuplicates = () => grouped
                    .Where(g => g.Count() > 1)
                    .SelectMany(g => from x in g select x.Name)
                    .OrderBy(x => x)
                    .Aggregate(Empty, (g, x) => IsNullOrEmpty(g) ? $"'{x}'" : $"{g}, '{x}'");
                outputHelper?.WriteLine($"Some Enumerated values have Duplicate Bits: {listDuplicates()}");
                throw;
            }
            // xunit: catch (EqualException)
            //{
            //    Func<string> listDuplicates = () => grouped
            //        .Where(g => g.Count() > 1)
            //        .SelectMany(g => from x in g select x.Name)
            //        .OrderBy(x => x)
            //        .Aggregate(Empty, (g, x) => IsNullOrEmpty(g) ? $"'{x}'" : $"{g}, '{x}'");
            //    outputHelper?.WriteLine($"Some Enumerated values have Duplicate Bits: {listDuplicates()}");
            //    throw;
            //}
        }
    }
}
