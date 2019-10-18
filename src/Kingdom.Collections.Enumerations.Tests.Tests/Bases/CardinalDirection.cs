namespace Kingdom.Collections.Bases
{
    public class CardinalDirection : BaseEnumeration<CardinalDirection>
    {
        protected internal override string DebuggerDisplayName => DisplayName;

        public static readonly CardinalDirection North = new CardinalDirection();

        public static readonly CardinalDirection NorthWest = new CardinalDirection();

        public static readonly CardinalDirection West = new CardinalDirection();

        public static readonly CardinalDirection SouthWest = new CardinalDirection();

        public static readonly CardinalDirection South = new CardinalDirection();

        public static readonly CardinalDirection SouthEast = new CardinalDirection();

        public static readonly CardinalDirection East = new CardinalDirection();

        public static readonly CardinalDirection NorthEast = new CardinalDirection();

        private CardinalDirection()
        {
        }
    }
}
