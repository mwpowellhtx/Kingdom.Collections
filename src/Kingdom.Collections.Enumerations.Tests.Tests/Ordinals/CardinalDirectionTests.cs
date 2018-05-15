using System.Collections.Generic;
using Xunit.Abstractions;

namespace Kingdom.Collections.Ordinals
{
    using Xunit;
    using static CardinalDirection;

    public class CardinalDirectionTests : OrdinalEnumerationTestsBase<CardinalDirection>
    {
        /// <summary>
        /// Life cycle management is critical here, especially with subtle differences between
        /// XUnit and NUnit run time approaches. XUnit will manage the life cycles of the
        /// injected parameters for us. Whereas, with NUnit, we should provide new'ed up
        /// instances to the base class ourselves.
        /// </summary>
        /// <param name="outputHelper"></param>
        /// <param name="reporter"></param>
        /// <inheritdoc />
        public CardinalDirectionTests(ITestOutputHelper outputHelper
            , EnumerationCoverageReporter<CardinalDirection> reporter)
            : base(outputHelper, reporter)
        {
        }

        public static readonly IEnumerable<object[]> TestValues;

        private static IEnumerable<object[]> GetTestValues()
        {
            var ordinal = 0;
            yield return new object[] {++ordinal, nameof(North), nameof(North)};
            yield return new object[] {++ordinal, nameof(West), nameof(West)};
            yield return new object[] {++ordinal, nameof(South), nameof(South)};
            yield return new object[] {++ordinal, nameof(East), nameof(East)};
        }

        static CardinalDirectionTests()
        {
            TestValues = GetTestValues();
        }

#pragma warning disable xUnit1008 // Test data attribute should only be used on a Theory
        [MemberData(nameof(TestValues))]
        public override void Verify_Enumeration_ordinal_value(int ordinal, string name, string displayName)
        {
            base.Verify_Enumeration_ordinal_value(ordinal, name, displayName);
        }
#pragma warning restore xUnit1008 // Test data attribute should only be used on a Theory

    }
}
