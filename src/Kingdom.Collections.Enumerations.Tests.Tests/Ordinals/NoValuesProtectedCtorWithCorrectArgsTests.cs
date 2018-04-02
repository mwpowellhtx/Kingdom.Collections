using System;

namespace Kingdom.Collections.Ordinals
{
    using NUnit.Framework;

    public class NoValuesProtectedCtorWithCorrectArgsTests
        : IndependentEnumerationTestBase<NoValuesProtectedCtorWithCorrectArgs>
    {
        [Test]
        public void Does_not_have_private_Ctor_with_correct_signature()
        {
            // Should throw the TrueException, not the NotNullException.
            // TODO: TBD: ditto thrown exception...
            ////Assert.Throws<Exception>(() => NullInstance.HasExpectedOrdinalCtors()); // nunit
            Assert.Throws<AssertionException>(() => NullInstance.HasExpectedOrdinalCtors()); // nunit
            //// TODO: TBD: this is where xunit can shine a little bit; we have a bit finer grain attitude control over thrown exceptions, etc...
            // xunit: Assert.Throws<TrueException>(() => NullInstance.HasExpectedOrdinalCtors());
        }
    }
}
