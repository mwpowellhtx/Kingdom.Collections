namespace Kingdom.Collections.Ordinals
{
    public class NoValuesProtectedCtorWithCorrectArgs : OrdinalEnumeration<NoValuesProtectedCtorWithCorrectArgs>
    {
        /// <summary>
        /// The Accessibility can be any other thing than Private here in order to demonstrate
        /// that we are testing for the right thing.
        /// </summary>
        /// <param name="ordinal"></param>
        /// <inheritdoc />
        protected NoValuesProtectedCtorWithCorrectArgs(int ordinal)
            : base(ordinal)
        {
        }
    }
}
