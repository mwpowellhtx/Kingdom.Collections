namespace Kingdom.Collections.Bitwises
{
    using Xunit;
    using Xunit.Sdk;

    public class ProtectedBitwiseCtorTests : IndependentEnumerationTestBase<ProtectedBitwiseCtor>
    {
        [Fact]
        public void The_expected_Ctor_is_not_Private()
        {
            Assert.Throws<TrueException>(() => NullInstance.HasExpectedBitwiseCtors());
        }

        [Fact]
        public void Expecting_at_least_One_value()
        {
            Assert.Throws<NotEmptyException>(() => NullInstance.ShallAllHaveConsistentBitLengths());
        }
    }
}
