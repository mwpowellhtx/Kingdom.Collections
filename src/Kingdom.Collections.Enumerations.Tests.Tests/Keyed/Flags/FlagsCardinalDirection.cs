using System.Collections.Generic;

namespace Kingdom.Collections.Keyed.Flags
{
    public partial class FlagsCardinalDirection : FlagsEnumerationBase<FlagsCardinalDirection>
    {
        public static readonly FlagsCardinalDirection North = new FlagsCardinalDirection();

        public static readonly FlagsCardinalDirection NorthWest = new FlagsCardinalDirection();

        public static readonly FlagsCardinalDirection West = new FlagsCardinalDirection();

        public static readonly FlagsCardinalDirection SouthWest = new FlagsCardinalDirection();

        public static readonly FlagsCardinalDirection South = new FlagsCardinalDirection();

        public static readonly FlagsCardinalDirection SouthEast = new FlagsCardinalDirection();

        public static readonly FlagsCardinalDirection East = new FlagsCardinalDirection();

        public static readonly FlagsCardinalDirection NorthEast = new FlagsCardinalDirection();

        static FlagsCardinalDirection()
        {
            InitializeValueKeys(Values, new ImmutableBitArray(0x1), x => x << 1);
        }

        private FlagsCardinalDirection()
        {
        }

        /// <inheritdoc />
        public override IEnumerable<FlagsCardinalDirection> EnumeratedValues => GetValues<FlagsCardinalDirection>();
    }
}
