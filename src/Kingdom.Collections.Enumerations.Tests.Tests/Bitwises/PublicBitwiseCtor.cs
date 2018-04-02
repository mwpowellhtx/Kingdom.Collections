namespace Kingdom.Collections.Bitwises
{
    public class PublicBitwiseCtor : Enumeration<PublicBitwiseCtor>
    {
        public PublicBitwiseCtor(byte[] bytes)
            : base(bytes)
        {
        }

        public static readonly PublicBitwiseCtor Null = null;

        public static readonly PublicBitwiseCtor First = new PublicBitwiseCtor(new byte[] {1});

        public static readonly PublicBitwiseCtor Duplicate = new PublicBitwiseCtor(new byte[] {1});

        public static readonly PublicBitwiseCtor Second = new PublicBitwiseCtor(new byte[] {2, 3});
    }
}
