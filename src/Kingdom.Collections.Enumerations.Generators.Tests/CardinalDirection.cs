namespace Kingdom.Collections
{
    [FlagsEnumeration]
    public partial class CardinalDirection : Enumeration<CardinalDirection>
    {
        public static readonly CardinalDirection North = new CardinalDirection();

        public static readonly CardinalDirection East = new CardinalDirection();

        public static readonly CardinalDirection South = new CardinalDirection();

        public static readonly CardinalDirection West = new CardinalDirection();

        static CardinalDirection() => InitializeBits(Values);

        private CardinalDirection()
        {
        }
    }
}
