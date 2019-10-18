using System.Collections.Generic;

namespace Kingdom.Collections.Bases
{
    using Xunit;
    using Xunit.Abstractions;
    using static CardinalDirection;

    public class CardinalDirectionTests : BaseEnumerationTestsBase<CardinalDirection>
        , IClassFixture<BaseEnumerationCoverageReporter<CardinalDirection>>
    {
        /// <summary>
        /// Life cycle management is critical here.
        /// </summary>
        /// <param name="outputHelper"></param>
        /// <param name="reporter"></param>
        /// <inheritdoc />
        public CardinalDirectionTests(ITestOutputHelper outputHelper
            , BaseEnumerationCoverageReporter<CardinalDirection> reporter)
            : base(outputHelper, reporter)
        {
        }

        public static readonly IEnumerable<object[]> TestValues;

        private static IEnumerable<object[]> GetTestValues()
        {
            // Make sure that these Extension methods are Internals Visible to this assembly.
            string GetDisplayName(string s) => s.GetHumanReadableCamelCase();

            yield return new object[] {nameof(North), nameof(North)};
            yield return new object[] {nameof(NorthWest), GetDisplayName(nameof(NorthWest))};
            yield return new object[] {nameof(West), nameof(West)};
            yield return new object[] {nameof(SouthWest), GetDisplayName(nameof(SouthWest))};
            yield return new object[] {nameof(South), nameof(South)};
            yield return new object[] {nameof(SouthEast), GetDisplayName(nameof(SouthEast))};
            yield return new object[] {nameof(East), nameof(East)};
            yield return new object[] {nameof(NorthEast), GetDisplayName(nameof(NorthEast))};
        }

        static CardinalDirectionTests()
        {
            TestValues = GetTestValues();
        }

#pragma warning disable xUnit1008 // Test data attribute should only be used on a Theory
        [MemberData(nameof(TestValues))]
        public override void Verify_Base_Enumeration_Names(string name, string displayName)
            => base.Verify_Base_Enumeration_Names(name, displayName);
#pragma warning restore xUnit1008 // Test data attribute should only be used on a Theory

    }
}
