using System;

namespace Kingdom.Collections.Bitwises
{
    using NUnit.Framework;

    public class PublicBitwiseCtorTests : IndependentEnumerationTestBase<PublicBitwiseCtor>
    {
        [Test]
        public void There_are_no_expected_Ctors()
        {
            // TODO: TBD: ditto thrown exception...
            Assert.Throws<AssertionException>(() => NullInstance.HasExpectedBitwiseCtors()); // nunit
            // xunit: Assert.Throws<NotNullException>(() => NullInstance.HasExpectedBitwiseCtors());
        }

        [Test]
        public void Expecting_all_values_to_be_Not_Null()
        {
            // TODO: TBD: ditto thrown exception...
            Assert.Throws<AssertionException>(() => NullInstance.ShallAllHaveConsistentBitLengths(false)); // nunit
            // xunit: Assert.Throws<NotNullException>(() => NullInstance.ShallAllHaveConsistentBitLengths(false));
        }

        [Test]
        public void Expecting_consistent_Bits_lengths()
        {
            // TODO: TBD: ditto thrown exception...
            Assert.Throws<AssertionException>(() => NullInstance.ShallAllHaveConsistentBitLengths()); // nunit
            // xunit: Assert.Throws<SingleException>(() => NullInstance.ShallAllHaveConsistentBitLengths());
        }

        [Test]
        public void Expecting_Values_to_be_uniquely_assigned()
        {
            // TODO: TBD: ditto thrown exception...
            Assert.Throws<AssertionException>(() => NullInstance.ValueBitsShallBeUniquelyAssigned()); // nunit
            // xunit: Assert.Throws<EqualException>(() => NullInstance.ValueBitsShallBeUniquelyAssigned());
        }
    }
}
