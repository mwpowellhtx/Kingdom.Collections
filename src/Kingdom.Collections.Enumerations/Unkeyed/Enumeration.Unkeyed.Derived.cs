using System.Collections.Generic;
using System.Linq;

namespace Kingdom.Collections
{
    /// <summary>
    /// Sometimes a strongly typed <see cref="Enumeration"/> may be required
    /// apart from <see cref="Enumeration{TKey,T}"/> Key details.
    /// </summary>
    /// <typeparam name="T">Represents the Derived Enumeration Type.</typeparam>
    public abstract class UnkeyedEnumeration<T> : Enumeration
        where T : UnkeyedEnumeration<T>
    {
        /// <summary>
        /// Protected Default Constructor.
        /// </summary>
        protected UnkeyedEnumeration()
        {
        }

        /// <summary>
        /// Protected Constructor.
        /// </summary>
        /// <param name="name"></param>
        protected UnkeyedEnumeration(string name)
            : base(name)
        {
        }

        /// <summary>
        /// Protected Constructor.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="displayName"></param>
        protected UnkeyedEnumeration(string name, string displayName)
            : base(name, displayName)
        {
        }

        /// <inheritdoc />
        protected override string ResolveName()
        {
            // TODO: TBD: may also want to restrict the field type(s) to those of the declaring type
            var field = GetDeclaringTypes(typeof(T))
                .SelectMany(type => type.GetFields(PublicStaticDeclaredOnly))
                .FirstOrDefault(fieldInfo => ReferenceEquals(this, fieldInfo.GetValue(null)));

            return field?.Name ?? string.Empty;
        }

        /// <summary>
        /// <see cref="Values"/> backing field.
        /// </summary>
        private static IEnumerable<T> _values;

        /// <summary>
        /// Gets the enumerated Values from the <typeparamref name="T"/> type.
        /// </summary>
        /// <see cref="Enumeration.GetValues{T}"/>
        public static IEnumerable<T> Values => _values ?? (_values = GetValues<T>().ToArray());
    }
}
