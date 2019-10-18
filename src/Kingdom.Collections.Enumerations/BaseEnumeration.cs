using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

namespace Kingdom.Collections
{
    using static BindingFlags;

    /// <summary>
    /// The Base Enumeration allows for a Free standing Enumeration, absent Ordinal
    /// or Flags style identification. It is presumed that <see cref="Name"/>, for
    /// instance, would be the unique, identifying information, but this is dependent
    /// upon the assumptions made in the application.
    /// </summary>
    [DebuggerDisplay("{" + nameof(DebuggerDisplayName) + "}")]
    public abstract class BaseEnumeration : IComparable<BaseEnumeration>, IComparable
        , IEquatable<BaseEnumeration>, IEquatable<object>
    {
        /// <summary>
        /// Provides a <see cref="DisplayName"/> for Debugging purposes.
        /// </summary>
        protected internal abstract string DebuggerDisplayName { get; }

        /// <summary>
        /// Name backing field.
        /// </summary>
        private string _name;

        /// <summary>
        /// Gets or sets the Name of the enum based on the <see cref="Type"/>.
        /// </summary>
        public string Name
        {
            get
            {
                TryResolveName(ref _name);
                return _name;
            }
            protected internal set => _name = value;
        }

        private IEnumerable<Type> GetDeclaringTypes()
        {
            var declaringTypes = GetType().GetEnumeratedValueTypes().ToArray();
            return GetDeclaringTypes(declaringTypes);
        }

        ////TODO: TBD: inspect for Properties as well as Fields? i.e. especially for so-called 'lazy' initialized? barring the fact we pretty much want to initialize as soon as at least one are to be referenced, and initialize for ordinals, bitwise, etc
        /// <summary>
        /// Defines an appropriate set of PublicStaticDeclaredOnly.
        /// </summary>
        /// <see cref="Public"/>
        /// <see cref="Static"/>
        /// <see cref="DeclaredOnly"/>
        protected const BindingFlags PublicStaticDeclaredOnly = Public | Static | DeclaredOnly;

        /// <summary>
        /// Returns the declaring types.
        /// </summary>
        /// <param name="declaringTypes"></param>
        /// <returns></returns>
        protected static IEnumerable<Type> GetDeclaringTypes(params Type[] declaringTypes)
        {
            var types = declaringTypes
                .SelectMany(type => type.GetNestedTypes(PublicStaticDeclaredOnly))
                .Where(type => type.IsClass && type.IsStatic()).ToList();

            declaringTypes.ToList().ForEach(type => types.Insert(0, type));

            return types;
        }

        /// <summary>
        /// Tries to resolve the Enumerated Name.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private bool TryResolveName(ref string value)
        {
            if (!string.IsNullOrEmpty(value))
            {
                return true;
            }

            //TODO: may also want to restrict the field type(s) to those of the declaring type
            var field = GetDeclaringTypes()
                .SelectMany(type => type.GetFields(PublicStaticDeclaredOnly))
                .FirstOrDefault(info => ReferenceEquals(this, info.GetValue(null)));

            value = field?.Name ?? string.Empty;

            return field != null
                   && !string.IsNullOrEmpty(value);
        }

        /// <summary>
        /// DisplayName backing field.
        /// </summary>
        private string _displayName;

        /// <summary>
        /// Gets the DisplayName.
        /// </summary>
        /// <see cref="Name"/>
        public string DisplayName
        {
            get
            {
                TryResolveHumanReadableCamelCase(ref _displayName, Name);
                return _displayName;
            }
            protected set => _displayName = value;
        }

        /// <summary>
        /// Tries to resolve a DisplayName.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        protected static bool TryResolveHumanReadableCamelCase(ref string value, string name)
        {
            if (value != null)
            {
                return true;
            }

            value = name.GetHumanReadableCamelCase();
            return !string.IsNullOrEmpty(value);
        }

        private static int CompareTo(BaseEnumeration a, BaseEnumeration b)
            => a?.Name == null
                ? -1
                : b?.Name == null
                    ? 1
                    : string.Compare(a.Name, b.Name, StringComparison.InvariantCultureIgnoreCase);

        /// <inheritdoc />
        public virtual int CompareTo(BaseEnumeration other) => CompareTo(this, other);

        /// <inheritdoc />
        public virtual int CompareTo(object other) => CompareTo(this, other as BaseEnumeration);

        /// <inheritdoc />
        public bool Equals(BaseEnumeration other) => CompareTo(this, other) == 0;

        /// <inheritdoc cref="IEquatable{T}"/>
        public override bool Equals(object other) => Equals(this, other as BaseEnumeration);

        /// <inheritdoc />
        public override int GetHashCode() => Name?.GetHashCode() ?? default(int);

        /// <summary>
        /// Default Protected Constructor.
        /// </summary>
        protected BaseEnumeration()
        {
        }

        /// <summary>
        /// Default Protected Constructor.
        /// </summary>
        /// <param name="name"></param>
        protected BaseEnumeration(string name)
            : this(name, null)
        {
        }

        /// <summary>
        /// Protected Constructor.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="displayName"></param>
        protected BaseEnumeration(string name, string displayName)
        {
            Name = name;
            DisplayName = displayName;
        }
    }
}