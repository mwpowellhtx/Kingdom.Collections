using System.Linq;

namespace Kingdom.Collections.Bitwises
{
    public partial class CardinalDirection : Enumeration<CardinalDirection>
    {
        public static readonly CardinalDirection North = new CardinalDirection();

        public static readonly CardinalDirection NorthWest = new CardinalDirection();

        public static readonly CardinalDirection West = new CardinalDirection();

        public static readonly CardinalDirection SouthWest = new CardinalDirection();

        public static readonly CardinalDirection South = new CardinalDirection();

        public static readonly CardinalDirection SouthEast = new CardinalDirection();

        public static readonly CardinalDirection East = new CardinalDirection();

        public static readonly CardinalDirection NorthEast = new CardinalDirection();

        static CardinalDirection()
        {
            var values = Values.ToArray();
            InitializeBits(values);
            InitializeBitsLengths(values, sizeof(int) * 8);
        }

        private CardinalDirection()
        {
        }
    }
}
