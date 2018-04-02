using System;
using System.Collections.Generic;
using System.Linq;

namespace Kingdom.Collections
{
    public abstract partial class Enumeration<TDerived>
    {
        /// <summary>
        /// Returns the BitwiseNot value from this instance.
        /// </summary>
        /// <returns></returns>
        /// <remarks>The only risk of supporting the bitwise not operator is to ensure that the
        /// bit masks are ubiquitous for a given set of enumerated values. Their lengths should
        /// all agree for this to make any sense at all.</remarks>
        protected virtual TDerived BitwiseNot()
        {
            // TODO: be careful with this one: converting to/from bytes is causing extra bits to be tacked on
            // TODO: which, when twos complemented, is actually inverting a whole nibble improperly
            // TODO: actually, I'm not sure how the lengths are ending up like they are to begin with

            // TODO: TBD: are we getting the value that we want? or are we getting a new one?
            // Be cautious that these are not operating in place.
            var result = FromBytes(Bits.ToBytes(false).ToArray());
            // Ensure that any extra padding that got tacked on is truncated.
            result.Bits = Bits.Not();
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="root"></param>
        /// <param name="other"></param>
        /// <param name="func"></param>
        /// <returns></returns>
        private static TDerived BitwiseFunc(TDerived root, TDerived other,
            Func<ImmutableBitArray, ImmutableBitArray, ImmutableBitArray> func)
        {
            // TODO: lengths may be an issue here ...
            var length = Math.Min(root.Bits.Length, other.Bits.Length);
            var funced = func(root.Bits, other.Bits);
            var result = FromBytes(funced.ToBytes(false).ToArray());
            result.Bits.Length = length;
            return result;
        }

        /// <summary>
        /// Returns the Bitwise Or value from this instance combined with
        /// <paramref name="other"/>.
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        protected internal virtual TDerived BitwiseOr(TDerived other)
            => BitwiseFunc(this as TDerived, other, (r, o) => r.Or(o));

        /// <summary>
        /// Returns the Bitwise And value from this instance combined with
        /// <paramref name="other"/>.
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        protected internal virtual TDerived BitwiseAnd(TDerived other)
            => BitwiseFunc(this as TDerived, other, (r, o) => r.And(o));

        /// <summary>
        /// Returns the Bitwise Exclusive Or value from this instance combined with
        /// <paramref name="other"/>.
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        protected internal virtual TDerived BitwiseXor(TDerived other)
            => BitwiseFunc(this as TDerived, other, (r, o) => r.Xor(o));

        /// <summary>
        /// Gets an <see cref="IEnumerable{PlayKind}"/> representing the Bitwise Decomposition of
        /// the Current Instance.
        /// </summary>
        /// <see cref="BitsLookup"/>
        /// <see cref="Enumeration.SetBitCount"/>
        public virtual IEnumerable<TDerived> EnumeratedValues
        {
            get
            {
                {
                    TDerived result;

                    /* This is the early return use case. Do not need to iterate the rest if we have this.
                     * Looking the value up is key, but so is ensuring this really has only one bit set.
                     * Otherwise, we may just look it up. */

                    if (SetBitCount <= 1 && BitsLookup.TryGetValue((TDerived) this, out result))
                    {
                        yield return result;
                        yield break;
                    }
                }

                // Otherwise we need to dive into the rest.
                foreach (var result in from v in Values
                    where v.SetBitCount >= 1 && v.Equals(BitwiseAnd(v))
                    select v)
                {
                    yield return result;
                }
            }
        }
    }
}
