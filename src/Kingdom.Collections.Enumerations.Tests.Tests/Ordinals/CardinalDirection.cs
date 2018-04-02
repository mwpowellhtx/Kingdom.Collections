namespace Kingdom.Collections.Ordinals
{
    public class CardinalDirection : OrdinalEnumeration<CardinalDirection>
    {
        public static readonly CardinalDirection North = new CardinalDirection(1);

        public static readonly CardinalDirection West = new CardinalDirection(2);

        public static readonly CardinalDirection South = new CardinalDirection(3);

        public static readonly CardinalDirection East = new CardinalDirection(4);

        private CardinalDirection(int ordinal)
            : base(ordinal)
        {
        }
    }
}
