using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using CodeGeneration.Roslyn;
using Validation;

namespace Kingdom.Collections
{
    using static BitwiseOperatorOverloadsPartialGenerator;
    using static Diagnostic;
    using static Requires;
    using static SyntaxFactory;
    using static Category;
    using static DiagnosticSeverity;

    /// <summary>
    ///
    /// </summary>
    // ReSharper disable once UnusedMember.Global
    public class FlagsEnumerationGenerator : ICodeGenerator
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="attributeData"></param>
        // ReSharper disable once UnusedMember.Global, UnusedParameter.Local
        public FlagsEnumerationGenerator(AttributeData attributeData)
        {
            NotNull(attributeData, nameof(attributeData));
        }

        private const string IdPrefix = "KingdomCollectionsEnumerations";
        private const string HelpUriBase = "https://github/mwpowellhtx/Kingdom.Collections/wiki/analyzers/rules/";

        private static DiagnosticDescriptor Rule(int id, string title, Category category
            , DiagnosticSeverity defaultSeverity, string messageFormat, string description = null)
        {
            const bool isEnabledByDefault = true;
            return new DiagnosticDescriptor($"{IdPrefix}{id}", title, messageFormat, $"{category}"
                , defaultSeverity, isEnabledByDefault, description, $"{HelpUriBase}{id}");
        }

        private static DiagnosticDescriptor X2000_FlagsEnumerationBitwiseOperatorsCodeGen { get; }
            = Rule(2000, "Flags Enumeration Bitwise Operators Code Generation", CodeGen
                , Error, "Generating Flags Enumeration Bitwise Operators code");

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <param name="progress"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task<SyntaxList<MemberDeclarationSyntax>> GenerateAsync(TransformationContext context
            , IProgress<Diagnostic> progress, CancellationToken cancellationToken)
        {
            progress.Report(Create(X2000_FlagsEnumerationBitwiseOperatorsCodeGen
                , context.ProcessingNode.GetLocation()));

            // We do not just expect a Member, but the Type, returned here.
            MemberDeclarationSyntax GenerateFlagsEnumerationPartial(FlagsEnumerationDescriptor d)
                => Generate(d, cancellationToken);

            return Task.Run(() =>
            {
                var generatedMembers = List<MemberDeclarationSyntax>();

                // ReSharper disable once InvertIf
                if (context.ProcessingNode is ClassDeclarationSyntax classDecl)
                {
                    var descriptor = classDecl.ToFlagsEnumerationDescriptor();
                    generatedMembers = generatedMembers.Add(GenerateFlagsEnumerationPartial(descriptor));
                }

                return generatedMembers;
            }, cancellationToken);
        }
    }
}
