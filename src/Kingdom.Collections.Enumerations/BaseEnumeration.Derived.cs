using System;
using System.Collections.Generic;
using System.Linq;

namespace Kingdom.Collections
{
    /// <summary>
    /// Provides the base class for a <typeparamref name="T"/> specific
    /// <see cref="BaseEnumeration"/>.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract partial class BaseEnumeration<T> : BaseEnumeration
        where T : BaseEnumeration<T>
    {
        /// <summary>
        /// Gets the Default <see cref="Enumeration"/>.
        /// </summary>
        public static T Default => Values.First();

        /// <summary>
        /// Values backing field.
        /// </summary>
        private static IEnumerable<T> _values;

        /// <summary>
        /// Gets the enumerated Values from the <typeparamref name="T"/> type.
        /// </summary>
        public static IEnumerable<T> Values => _values ?? (_values = GetValues());

        // TODO: TBD: we may look at doing this from a shared utility method...
        // TODO: TBD: which would also serve the Enumeration<T> class ...
        /// <summary>
        /// Returns the enumerated Values.
        /// </summary>
        /// <param name="ignoreNulls"></param>
        /// <returns></returns>
        public static IEnumerable<T> GetValues(bool ignoreNulls = true)
        {
            // Keep the formatting and implementation this way for troubleshooting purposes.
            var declaringTypes = GetDeclaringTypes(typeof(T)).Reverse().ToArray();

            // TODO: TBD: determine how to treat Null values... at the moment, it seems as though Null is being ignored...
            foreach (var values in declaringTypes
                .Select(t => t.GetFields(PublicStaticDeclaredOnly))
                .Select(fis => fis.Select(info => info.GetValue(null)).ToArray()))
            {
                foreach (var value in values.Where(x => !ignoreNulls || x != null))
                {
                    yield return (T) value;
                }

                //// TODO: TBD: this may work, but I'm not sure, if memory serves, there was a problem with this approach?
                //foreach (var value in values.OfType<TDerived>())
                //{
                //    yield return value;
                //}
            }
        }

        /// <summary>
        /// Protected Default Constructor.
        /// </summary>
        protected BaseEnumeration()
        {
        }

        /// <summary>
        /// Protected Constructor.
        /// </summary>
        /// <param name="displayName"></param>
        protected BaseEnumeration(string displayName)
            : base(null, displayName)
        {
        }

        /// <summary>
        /// Protected Constructor.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="displayName"></param>
        protected BaseEnumeration(string name, string displayName)
            : base(name, displayName)
        {
        }

        private static T FromName(IEnumerable<T> values, string name, StringComparison? comparison = null)
            => comparison.HasValue
                ? values.FirstOrDefault(x => string.Equals(x.Name, name, comparison.Value))
                : values.FirstOrDefault(x => string.Equals(x.Name, name));

        /// <summary>
        /// Returns the <typeparamref name="T"/> <see cref="BaseEnumeration{T}"/>
        /// given the <paramref name="name"/>. Optionally provide a
        /// <paramref name="comparison"/>.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="comparison"></param>
        /// <returns></returns>
        public static T FromName(string name, StringComparison? comparison = null) => FromName(Values, name, comparison);

        private static T FromDisplayName(IEnumerable<T> values, string displayName, StringComparison? comparison = null)
            => comparison.HasValue
                ? values.FirstOrDefault(x => string.Equals(x.DisplayName, displayName, comparison.Value))
                : values.FirstOrDefault(x => string.Equals(x.DisplayName, displayName));

        /// <summary>
        /// Returns the <typeparamref name="T"/> <see cref="BaseEnumeration{T}"/>
        /// given the <paramref name="displayName"/>. Optionally provide a
        /// <paramref name="comparison"/>.
        /// </summary>
        /// <param name="displayName"></param>
        /// <param name="comparison"></param>
        /// <returns></returns>
        public static T FromDisplayName(string displayName, StringComparison? comparison = null) => FromDisplayName(Values, displayName, comparison);
    }
}
