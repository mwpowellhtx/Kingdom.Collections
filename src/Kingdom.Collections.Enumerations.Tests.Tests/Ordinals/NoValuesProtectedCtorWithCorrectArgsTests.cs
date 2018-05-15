namespace Kingdom.Collections.Ordinals
{
    using Xunit;
    using Xunit.Sdk;

    public class NoValuesProtectedCtorWithCorrectArgsTests
        : IndependentEnumerationTestBase<NoValuesProtectedCtorWithCorrectArgs>
    {
        [Fact]
        public void Does_not_have_private_Ctor_with_correct_signature()
        {
            // Should throw the TrueException, not the NotNullException.
            Assert.Throws<TrueException>(() => NullInstance.HasExpectedOrdinalCtors());
        }
    }
}
