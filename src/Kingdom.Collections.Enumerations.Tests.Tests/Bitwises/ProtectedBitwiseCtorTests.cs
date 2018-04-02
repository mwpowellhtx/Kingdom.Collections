using System;

namespace Kingdom.Collections.Bitwises
{
    using NUnit.Framework;

    public class ProtectedBitwiseCtorTests : IndependentEnumerationTestBase<ProtectedBitwiseCtor>
    {
        [Test]
        public void The_expected_Ctor_is_not_Private()
        {
            // TODO: TBD: may specialize the thrown exception here for NUnit purposes...
            Assert.Throws<AssertionException>(() => NullInstance.HasExpectedBitwiseCtors()); // nunit
            // xunit: Assert.Throws<TrueException>(() => NullInstance.HasExpectedBitwiseCtors());
        }

        [Test]
        public void Expecting_at_least_One_value()
        {
            // TODO: TBD: ditto...
            Assert.Throws<AssertionException>(() => NullInstance.ShallAllHaveConsistentBitLengths()); // nunit
            // xunit: Assert.Throws<NotEmptyException>(() => NullInstance.ShallAllHaveConsistentBitLengths());
        }
    }
}
