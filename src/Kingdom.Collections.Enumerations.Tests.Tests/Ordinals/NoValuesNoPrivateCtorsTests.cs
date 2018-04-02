using System;

namespace Kingdom.Collections.Ordinals
{
    using NUnit.Framework;

    /// <summary>
    /// This set of unit tests asserts the negative consequences of a badly formed Enumeration.
    /// Chiefly, the <see cref="NoValuesNoPrivateCtors"/>
    /// <see cref="OrdinalEnumeration{TDerived}"/> has no Values, no expected Private Constructor,
    /// and does have a Public Constructor.
    /// </summary>
    public class NoValuesNoPrivateCtorsTests : IndependentEnumerationTestBase<NoValuesNoPrivateCtors>
    {
        [Test]
        public void Does_not_have_expected_Ctors()
        {
            // TODO: TBD: ditto thrown exception...
            Assert.Throws<AssertionException>(() => NullInstance.HasExpectedOrdinalCtors()); // nunit
            // xunit: Assert.Throws<NotNullException>(() => NullInstance.HasExpectedOrdinalCtors());
        }

        [Test]
        public void Does_not_have_any_Values()
        {
            // TODO: TBD: ditto thrown exception...
            Assert.Throws<AssertionException>(() => NullInstance.ShallHaveAtLeastOneValue()); // nunit
            // xunit: Assert.Throws<TrueException>(() => NullInstance.ShallHaveAtLeastOneValue());
        }

        [Test]
        public void Does_have_one_Public_Ctor()
        {
            // TODO: TBD: ditto thrown exception...
            Assert.Throws<AssertionException>(() => NullInstance.ShallNotHaveAnyPublicCtors()); // nunit
            // xunit: Assert.Throws<EmptyException>(() => NullInstance.ShallNotHaveAnyPublicCtors());
        }
    }
}
