namespace Kingdom.Collections
{
    /// <summary>
    /// We will expect that actual Code Generation occurs for this definition.
    /// Which, there should be artifacts of that Code Generation vetted via
    /// <see cref="InSituFlagsEnumerationCodeGenerationTestsBase{T}"/>.
    /// </summary>
    /// <inheritdoc />
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
