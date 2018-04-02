namespace Kingdom.Collections.Bitwises
{
    public partial class CardinalDirection
    {
        private CardinalDirection(byte[] bytes)
            : base(bytes)
        {
        }

        public static CardinalDirection operator &(CardinalDirection a, CardinalDirection b) => a?.BitwiseAnd(b);

        public static CardinalDirection operator |(CardinalDirection a, CardinalDirection b) => a?.BitwiseOr(b);

        public static CardinalDirection operator ^(CardinalDirection a, CardinalDirection b) => a?.BitwiseXor(b);

        public static CardinalDirection operator ~(CardinalDirection other) => other?.BitwiseNot();
    }
}
