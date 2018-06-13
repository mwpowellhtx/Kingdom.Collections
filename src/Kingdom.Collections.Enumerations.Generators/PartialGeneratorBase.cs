using System.Threading;
using Microsoft.CodeAnalysis.CSharp;

namespace Kingdom.Collections
{
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using static SyntaxFactory;
    using static SyntaxKind;

    internal abstract class PartialGeneratorBase
    {
        protected FlagsEnumerationDescriptor Descriptor { get; }

        protected CancellationToken CancellationToken { get; }

        protected PartialGeneratorBase(FlagsEnumerationDescriptor descriptor
            , CancellationToken cancellationToken)
        {
            Descriptor = descriptor;
            CancellationToken = cancellationToken;
        }

        public virtual ClassDeclarationSyntax GenerateTypeDeclaration()
            => ClassDeclaration(GenerateTypeIdentifier())
                .WithTypeParameterList(GenerateTypeParameterList())
                .WithBaseList(GenerateBaseList())
                .WithModifiers(GenerateModifiers())
                .WithMembers(GenerateMembers());

        protected virtual TypeParameterListSyntax GenerateTypeParameterList()
            => Descriptor.TypeDecl.TypeParameterList?.WithoutTrivia();

        protected virtual SyntaxToken GenerateTypeIdentifier()
            => Descriptor.TypeIdentifier.WithoutTrivia();

        protected virtual SyntaxTokenList GenerateModifiers()
            => TokenList(Token(PartialKeyword));

        protected virtual SyntaxList<MemberDeclarationSyntax> GenerateMembers()
            => List<MemberDeclarationSyntax>();

        protected virtual BaseListSyntax GenerateBaseList() => null;
    }
}
