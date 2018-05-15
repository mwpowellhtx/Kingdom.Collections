namespace Kingdom.Collections.Ordinals
{
    using Xunit;
    using Xunit.Sdk;

    /// <summary>
    /// This set of unit tests asserts the negative consequences of a badly formed Enumeration.
    /// Chiefly, the <see cref="NoValuesNoPrivateCtors"/>
    /// <see cref="OrdinalEnumeration{TDerived}"/> has no Values, no expected Private Constructor,
    /// and does have a Public Constructor.
    /// </summary>
    public class NoValuesNoPrivateCtorsTests : IndependentEnumerationTestBase<NoValuesNoPrivateCtors>
    {
        [Fact]
        public void Does_not_have_expected_Ctors()
        {
            Assert.Throws<NotNullException>(() => NullInstance.HasExpectedOrdinalCtors());
        }

        [Fact]
        public void Does_not_have_any_Values()
        {
            Assert.Throws<TrueException>(() => NullInstance.ShallHaveAtLeastOneValue());
        }

        [Fact]
        public void Does_have_one_Public_Ctor()
        {
            Assert.Throws<EmptyException>(() => NullInstance.ShallNotHaveAnyPublicCtors());
        }
    }
}
