//using System.Collections.Generic;
//using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Kingdom.Collections
{
    using static SyntaxFactory;

    internal partial class FlagsEnumerationDescriptor
    {
        internal TypeSyntax Type { get; }

        internal SyntaxToken TypeIdentifier { get; }

        //private ImmutableArray<Entry> Entries { get; }

        internal TypeDeclarationSyntax TypeDecl { get; }

        /// <summary>
        /// Returns a Created instance of the <see cref="FlagsEnumerationDescriptor"/> based on
        /// the given parameters.  Furthermore, as far as I could determine, this can all be
        /// derived from the <paramref name="typeDecl"/> itself.
        /// </summary>
        /// <param name="typeDecl"></param>
        /// <returns></returns>
        public static FlagsEnumerationDescriptor Create(TypeDeclarationSyntax typeDecl)
            => new FlagsEnumerationDescriptor(typeDecl);

#pragma warning disable CS1574 // XML comment has cref attribute that could not be resolved
        /// <summary>
        /// Derived <see cref="NameSyntax"/> Type Syntax based on the <paramref name="typeDecl"/>,
        /// Identifier <see cref="SyntaxToken"/>, as well as the base <paramref name="typeDecl"/>.
        /// We do so in a way that hides the
        /// <see cref="SyntaxNodeExtensions.WithoutTrivia{TSyntax}"/> from the caller in a
        /// transparent manner.
        /// </summary>
        /// <param name="typeDecl"></param>
        private FlagsEnumerationDescriptor(TypeDeclarationSyntax typeDecl)
        {
            Type = typeDecl.GetTypeSyntax().WithoutTrivia();
            TypeIdentifier = typeDecl.Identifier.WithoutTrivia();
            TypeDecl = typeDecl.WithoutTrivia();
        }
#pragma warning restore CS1574 // XML comment has cref attribute that could not be resolved

        //internal abstract class Entry
        //{
        //    protected Entry(SyntaxToken identifier, TypeSyntax type, PropertyDeclarationSyntax syntax)
        //    {
        //        Identifier = identifier.WithoutTrivia();
        //        Type = type.WithoutTrivia();
        //        Syntax = syntax.WithoutTrivia();
        //    }
        //    internal SyntaxToken Identifier { get; }
        //    internal TypeSyntax Type { get; }
        //    internal PropertyDeclarationSyntax Syntax { get; }
        //}

        //internal class SimpleEntry : Entry
        //{
        //    public SimpleEntry(SyntaxToken identifier, TypeSyntax type, PropertyDeclarationSyntax syntax)
        //        : base(identifier, type, syntax)
        //    {
        //    }
        //}
    }

    internal static class FlagsEnumerationDescriptorExtensions
    {
        //public static FlagsEnumerationDescriptor.Entry ToFlagsEnumerationEntry(this PropertyDeclarationSyntax property)
        //    => new FlagsEnumerationDescriptor.SimpleEntry(property.Identifier, property.Type, property);

        public static FlagsEnumerationDescriptor ToFlagsEnumerationDescriptor(this ClassDeclarationSyntax classDecl)
            => FlagsEnumerationDescriptor.Create(classDecl);

        //// TODO: TBD: is "struct" really necessary? in this case, probably not, but will factor it out as I close in on an appropriate Enumerations solution...
        //public static FlagsEnumerationDescriptor ToFlagsEnumerationDescriptor(this StructDeclarationSyntax structDecl)
        //{
        //    return new FlagsEnumerationDescriptor(
        //        structDecl.GetTypeSyntax().WithoutTrivia()
        //        , structDecl.Identifier.WithoutTrivia()
        //        //, structDecl.GetRecordProperties()
        //        , structDecl.WithoutTrivia()
        //    );
        //}

        //private static ImmutableArray<FlagsEnumerationDescriptor.Entry> GetRecordProperties(
        //    this TypeDeclarationSyntax typeDecl)
        //    => typeDecl.Members.GetRecordProperties().AsRecordEntries();

        //private static ImmutableArray<PropertyDeclarationSyntax> GetRecordProperties(
        //    this SyntaxList<MemberDeclarationSyntax> members)
        //    => members.OfType<PropertyDeclarationSyntax>()
        //        .Where(propSyntax => propSyntax.IsPropertyViable()).ToImmutableArray();

        //private static ImmutableArray<FlagsEnumerationDescriptor.Entry> AsRecordEntries(
        //this IEnumerable<PropertyDeclarationSyntax> properties)
        //=> properties
        //    .Select(p => p.ToFlagsEnumerationEntry()).ToImmutableArray();

        public static QualifiedNameSyntax ToNestedBuilderType(this NameSyntax type)
            => QualifiedName(type, IdentifierName(Names.Builder));

        public static SyntaxToken ToLowerFirstLetter(this SyntaxToken identifier)
            => Identifier(identifier.Text.ToLowerFirstLetter());

        public static string ToLowerFirstLetter(this string name)
            => string.IsNullOrEmpty(name)
                ? name
                : $"{char.ToLowerInvariant(name.First())}{name.Substring(1)}";
    }
}
