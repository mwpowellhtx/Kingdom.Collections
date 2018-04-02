using System;
using System.Collections.Generic;
using System.Linq;

namespace Kingdom.Collections.Bitwises
{
    using NUnit.Framework;
    using static CardinalDirection;

    public class CardinalDirectionTests : BitwiseEnumerationTestsBase<CardinalDirection>
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
            Func<int, byte[]> next = x => new[] {(byte) (1 << x)};
            Func<string, string> getDisplayName = s => s.GetHumanReadableCamelCase();
            var shift = -1;
            yield return new object[] {next(++shift), nameof(North), nameof(North)};
            yield return new object[] {next(++shift), nameof(NorthWest), getDisplayName(nameof(NorthWest))};
            yield return new object[] {next(++shift), nameof(West), nameof(West)};
            yield return new object[] {next(++shift), nameof(SouthWest), getDisplayName(nameof(SouthWest))};
            yield return new object[] {next(++shift), nameof(South), nameof(South)};
            yield return new object[] {next(++shift), nameof(SouthEast), getDisplayName(nameof(SouthEast))};
            yield return new object[] {next(++shift), nameof(East), nameof(East)};
            yield return new object[] {next(++shift), nameof(NorthEast), getDisplayName(nameof(NorthEast))};
        }

        static CardinalDirectionTests()
        {
            TestValues = GetTestValues()
                    .Select(x => new TestCaseData(x)) // nunit
                ;
        }

#pragma warning disable xUnit1008 // Test data attribute should only be used on a Theory
        [Test, TestCaseSource(nameof(TestValues))] // nunit
        // xunit: [MemberData(nameof(TestValues))]
        public override void Verify_Bitwise_Enumeration_bits(byte[] bytes, string name, string displayName)
        {
            base.Verify_Bitwise_Enumeration_bits(bytes, name, displayName);
        }
#pragma warning restore xUnit1008 // Test data attribute should only be used on a Theory

    }
}
