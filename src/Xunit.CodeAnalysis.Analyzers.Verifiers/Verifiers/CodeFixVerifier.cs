using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Microsoft.CodeAnalysis;

namespace Xunit.CodeAnalysis
{
    using Abstractions;
    using Microsoft.CodeAnalysis.CodeActions;
    using Microsoft.CodeAnalysis.CodeFixes;
    using Microsoft.CodeAnalysis.Diagnostics;
    using Microsoft.CodeAnalysis.Formatting;
    using Sdk;
    using Xunit;
    using static LanguageNames;
    using static String;

    public partial class CodeFixVerifier : DiagnosticVerifier
    {
        // TODO: TBD: for now, we need both constructors; however, the pattern will shift a bit in coming version(s) of Xunit.
        /// <summary>
        /// Protected Constructor
        /// </summary>
        /// <param name="outputHelper"></param>
        /// <inheritdoc />
        // ReSharper disable once UnusedMember.Global
        protected CodeFixVerifier(ITestOutputHelper outputHelper)
            : base(outputHelper)
        {
        }

        /// <summary>
        /// Protected Constructor
        /// </summary>
        /// <inheritdoc />
        // ReSharper disable once UnusedMember.Global
        protected CodeFixVerifier()
        {
        }

        /// <summary>
        /// Returns the CSharp Code Fix being tested.
        /// </summary>
        /// <returns>The CodeFixProvider to be used for CSharp code.</returns>
        protected virtual CodeFixProvider GetCSharpCodeFixProvider() => null;

        /// <summary>
        /// Override to returns the VB Code Fix being tested.
        /// </summary>
        /// <returns>The CodeFixProvider to be used for VB code.</returns>
        protected virtual CodeFixProvider GetBasicCodeFixProvider() => null;

        private static void VerifyGivenSource(string source)
        {
            // Given Source cannot be Null, but it may be Empty.
            Assert.NotNull(source);
        }

        /// <summary>
        /// Called to test a CSharp Code Fix when applied on the inputted string as a source.
        /// The method short circuits when there is not an <paramref name="expectedSource"/>.
        /// </summary>
        /// <param name="givenSource">A class in the form of a string before the Code Fix was
        /// applied to it.</param>
        /// <param name="expectedSource">A class in the form of a string after the Code Fix was
        /// applied to it.</param>
        /// <param name="codeFixIndex">Index determining which Code Fix to apply if there are
        /// multiple.</param>
        /// <param name="allowNewCompilerDiagnostics">A bool controlling whether or not the
        /// test will fail if the Code Fix introduces other warnings after being applied.</param>
        public void VerifyCSharpFix(string givenSource, string expectedSource
            , int? codeFixIndex = null, bool allowNewCompilerDiagnostics = false)
        {
            VerifyGivenSource(givenSource);

            // TODO: TBD We'll keep the VB version around for now...
            // TODO: TBD: however, for my purposes, I don't care that much about maintaining VB, never mind introducing an FSharp version...
            if (IsNullOrEmpty(expectedSource))
            {
                OutputHelper.WriteLine($"Unable to verify without \"{nameof(expectedSource)}\".");
                return;
            }

            OutputHelper.WriteLine($"<given>\r\n{givenSource}\r\n</given>\r\n");
            OutputHelper.WriteLine($"<expected>\r\n{expectedSource}\r\n</expected>");

            // Run the Verification when we have Expected as well as Given.
            VerifyFix(CSharp, GetCSharpDiagnosticAnalyzer(), GetCSharpCodeFixProvider()
                , givenSource, expectedSource, codeFixIndex, allowNewCompilerDiagnostics);
        }

        /// <summary>
        /// Called to test a VB Code Fix when applied on the inputted string as a source.
        /// </summary>
        /// <param name="givenSource">A class in the form of a string before the Code Fix was
        /// applied to it.</param>
        /// <param name="expectedSource">A class in the form of a string after the Code Fix was
        /// applied to it.</param>
        /// <param name="codeFixIndex">Index determining which Code Fix to apply if there
        /// are multiple.</param>
        /// <param name="allowNewCompilerDiagnostics">A bool controlling whether or not the test
        /// will fail if the Code Fix introduces other warnings after being applied.</param>
        // ReSharper disable once UnusedMember.Global
        protected void VerifyBasicFix(string givenSource, string expectedSource
            , int? codeFixIndex = null, bool allowNewCompilerDiagnostics = false)
        {
            VerifyGivenSource(givenSource);

            if (IsNullOrEmpty(expectedSource))
            {
                OutputHelper.WriteLine($"Unable to verify without \"{nameof(expectedSource)}\".");
                return;
            }

            OutputHelper.WriteLine($"<given>\r\n{givenSource}\r\n</given>\r\n");
            OutputHelper.WriteLine($"<expected>\r\n{expectedSource}\r\n</expected>");

            // Run the Verification when we have Expected as well as Given.
            VerifyFix(VisualBasic, GetCSharpDiagnosticAnalyzer(), GetCSharpCodeFixProvider()
                , givenSource, expectedSource, codeFixIndex, allowNewCompilerDiagnostics);
        }

        /// <summary>
        /// General verifier for Code Fixes. Creates a Document from the source string, then
        /// gets diagnostics on it and applies the relevant Code Fixes. Then gets the string
        /// after the code fix is applied and compares it with the expected result. Note, if
        /// any code fix causes new diagnostics to show up, the test fails unless
        /// <paramref name="allowNewCompilerDiagnostics"/> is set to true.
        /// </summary>
        /// <param name="language">The language the source code is in.</param>
        /// <param name="analyzer">The analyzer to be applied to the source code.</param>
        /// <param name="codeFixProvider">The Code Fix to be applied to the code wherever
        /// the relevant Diagnostic is found.</param>
        /// <param name="givenSource">A class in the form of a string before the Code Fix was
        /// applied to it.</param>
        /// <param name="expectedSource">A class in the form of a string after the Code Fix was
        /// applied to it.</param>
        /// <param name="codeFixIndex">Index determining which Code Fix to apply if there are
        /// multiple.</param>
        /// <param name="allowNewCompilerDiagnostics">A bool controlling whether or not the
        /// test will fail if the Code Fix introduces other warnings after being applied.</param>
        private void VerifyFix(string language, DiagnosticAnalyzer analyzer, CodeFixProvider codeFixProvider
            , string givenSource, string expectedSource, int? codeFixIndex, bool allowNewCompilerDiagnostics)
        {
            var document = CreateDocument(givenSource, language);
            var analyzerDiagnostics = GetSortedDiagnosticsFromDocuments(analyzer, document);
            var compilerDiagnostics = GetCompilerDiagnostics(document).ToArray();
            // ReSharper disable once PossibleMultipleEnumeration
            var attempts = analyzerDiagnostics.Count();

            // ReSharper disable once PossibleMultipleEnumeration
            // Check if there are analyzer diagnostics left after the code fix.
            for (var i = 0; i < attempts && analyzerDiagnostics.Any(); ++i)
            {
                var actions = new List<CodeAction>();
                // ReSharper disable once PossibleMultipleEnumeration
                var context = new CodeFixContext(document, analyzerDiagnostics.First()
                    , (a, d) => actions.Add(a), CancellationToken.None);

                codeFixProvider.RegisterCodeFixesAsync(context).Wait();

                if (!actions.Any())
                {
                    break;
                }

                if (codeFixIndex != null)
                {
                    document = ApplyFix(document, actions.ElementAt((int) codeFixIndex));
                    break;
                }

                document = ApplyFix(document, actions.ElementAt(0));
                analyzerDiagnostics = GetSortedDiagnosticsFromDocuments(analyzer, document);

                var newCompilerDiagnostics = GetNewDiagnostics(compilerDiagnostics
                    , GetCompilerDiagnostics(document)).SortDiagnostics().ToArray();

                try
                {
                    // Check if applying the code fix introduced any new compiler diagnostics.
                    Assert.True(allowNewCompilerDiagnostics || !newCompilerDiagnostics.Any());
                }
                catch (TrueException)
                {
                    // Format and get the compiler diagnostics again so that the locations make sense in the output.
                    document = document.WithSyntaxRoot(Formatter.Format(document.GetSyntaxRootAsync().Result
                        , Formatter.Annotation, document.Project.Solution.Workspace));

                    newCompilerDiagnostics = GetNewDiagnostics(compilerDiagnostics
                        , GetCompilerDiagnostics(document)).SortDiagnostics().ToArray();

                    OutputHelper.WriteLine(
                        "Fix introduced new compiler diagnostics:"
                        + $"\r\n{Join("\r\n", newCompilerDiagnostics.Select(d => $"{d}"))}"
                        + $"\r\n\r\nNew document:\r\n{document.GetSyntaxRootAsync().Result.ToFullString()}");

                    throw;
                }
            }

            // After applying all of the Code Fixes, compare the resulting string to the inputted one.
            var fixedSource = GetStringFromDocument(document);
            Assert.Equal(expectedSource, fixedSource);
        }
    }
}
