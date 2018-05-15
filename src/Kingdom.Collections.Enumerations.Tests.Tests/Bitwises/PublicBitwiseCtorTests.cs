namespace Kingdom.Collections.Bitwises
{
    using Xunit;
    using Xunit.Sdk;

    public class PublicBitwiseCtorTests : IndependentEnumerationTestBase<PublicBitwiseCtor>
    {
        [Fact]
        public void There_are_no_expected_Ctors()
        {
            Assert.Throws<NotNullException>(() => NullInstance.HasExpectedBitwiseCtors());
        }

        [Fact]
        public void Expecting_all_values_to_be_Not_Null()
        {
            Assert.Throws<NotNullException>(() => NullInstance.ShallAllHaveConsistentBitLengths(false));
        }

        [Fact]
        public void Expecting_consistent_Bits_lengths()
        {
            Assert.Throws<SingleException>(() => NullInstance.ShallAllHaveConsistentBitLengths());
        }

        [Fact]
        public void Expecting_Values_to_be_uniquely_assigned()
        {
            Assert.Throws<EqualException>(() => NullInstance.ValueBitsShallBeUniquelyAssigned());
        }
    }
}
