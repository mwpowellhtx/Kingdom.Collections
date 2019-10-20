namespace Kingdom.Collections.Independents
{
    public class ProtectedBitwiseCtor : FlagsEnumerationBase<ProtectedBitwiseCtor>
    {
        // ReSharper disable once ParameterTypeCanBeEnumerable.Local
        protected ProtectedBitwiseCtor(byte[] bytes)
            : base(bytes)
        {
        }
    }
}
