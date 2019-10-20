namespace Kingdom.Collections.Unkeyed
{
    public class UnkeyedCardinalDirection : UnkeyedEnumeration<UnkeyedCardinalDirection>
    {
        public static readonly UnkeyedCardinalDirection North = new UnkeyedCardinalDirection();

        public static readonly UnkeyedCardinalDirection West = new UnkeyedCardinalDirection();

        public static readonly UnkeyedCardinalDirection South = new UnkeyedCardinalDirection();

        public static readonly UnkeyedCardinalDirection East = new UnkeyedCardinalDirection();

        private UnkeyedCardinalDirection()
        {
        }
    }
}
