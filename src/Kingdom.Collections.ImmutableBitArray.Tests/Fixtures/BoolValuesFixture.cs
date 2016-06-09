namespace Kingdom.Collections
{
    public class BoolValuesFixture : ValuesFixtureBase<bool>
    {
        protected override string ToString(bool value)
        {
            return value ? "1" : "0";
        }

        public BoolValuesFixture()
            : base(() => 1)
        {
        }
    }
}
