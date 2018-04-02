using System;
using System.Collections.Generic;
using System.Linq;

namespace Kingdom.Collections.Ordinals
{
    using NUnit.Framework;
    using static CardinalDirection;

    public class CardinalDirectionTests : OrdinalEnumerationTestsBase<CardinalDirection>
    {
        //// xunit
        //public CardinalDirectionTests(ITestOutputHelper outputHelper, EnumerationCoverageReporter<CardinalDirection> reporter)
        //    : base(outputHelper, reporter)
        //{
        //}

        // nunit
        public CardinalDirectionTests()
            : base(new TestOutputHelper(), new EnumerationCoverageReporter<CardinalDirection>())
        {
        }

        // xunit: public static readonly IEnumerable<object[]> TestValues;
        public static readonly IEnumerable<TestCaseData> TestValues; // nunit

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
            TestValues = GetTestValues() // nunit/xunit
                    .Select(x => new TestCaseData(x)).ToArray() // nunit
                ;
        }

#pragma warning disable xUnit1008 // Test data attribute should only be used on a Theory
        [Test, TestCaseSource(nameof(TestValues))] // nunit
        // xunit: [MemberData(nameof(TestValues))]
        public override void Verify_Enumeration_ordinal_value(int ordinal, string name, string displayName)
        {
            base.Verify_Enumeration_ordinal_value(ordinal, name, displayName);
        }
#pragma warning restore xUnit1008 // Test data attribute should only be used on a Theory

    }
}
