using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;

namespace Xunit.CodeAnalysis
{
    using Abstractions;
    using Microsoft.CodeAnalysis.Diagnostics;
    using Sdk;
    using Xunit;
    using static LanguageNames;

    // TODO: TBD: really, these could do well as a wholely separate assembly, perhaps even abstract out some key parts into a core assembly...
    public abstract partial class DiagnosticVerifier
    {
        // TODO: TBD: ditto the concerns noted in the ctor; it is a little more exposed than I would like it to be ATM...
        /// <summary>
        /// Gets or sets the OutputHelper.
        /// </summary>
        public ITestOutputHelper OutputHelper { get; set; }

        // TODO: TBD: ideally, I'd like to see this, but I do not thing Xunit is quite "there" yet...
        // see: ITestOutputHelper in call context #713 / http://github.com/xunit/xunit/issues/713
        // see: Current test status #621 / http://github.com/xunit/xunit/issues/621
        /// <summary>
        /// Protected Constructor
        /// </summary>
        /// <param name="outputHelper">The output helper.</param>
        protected DiagnosticVerifier(ITestOutputHelper outputHelper)
        {
            OutputHelper = outputHelper;
        }

        /// <summary>
        /// Protected Constructor
        /// </summary>
        protected DiagnosticVerifier()
        {
        }

        #region To be implemented by Test classes

        /// <summary>
        /// Override to return the CSharp analyzer being tested.
        /// </summary>
        protected virtual DiagnosticAnalyzer GetCSharpDiagnosticAnalyzer() => null;

        /// <summary>
        /// Override to return the VB analyzer being tested.
        /// </summary>
        protected virtual DiagnosticAnalyzer GetBasicDiagnosticAnalyzer() => null;

        #endregion

        #region Verifier wrappers

        /// <summary>
        /// Called to test a CSharp <see cref="DiagnosticAnalyzer"/> when applied on the single
        /// inputted string as a source. Note, input a <see cref="DiagnosticResult"/> for each
        /// Diagnostic expected
        /// </summary>
        /// <param name="source">A class in the form of a string to run the analyzer on.</param>
        /// <param name="expected"><see cref="DiagnosticResult"/> that should appear after the
        /// analyzer is run on the source.</param>
        public void VerifyCSharpDiagnostic(string source, params DiagnosticResult[] expected)
        {
            VerifyDiagnostics(new[] {source}, CSharp, GetCSharpDiagnosticAnalyzer(), expected);
        }

        /// <summary>
        /// Called to test a VB <see cref="DiagnosticAnalyzer"/> when applied on the single
        /// inputted string as a source. Note, input a <see cref="DiagnosticResult"/> for each
        /// Diagnostic expected.
        /// </summary>
        /// <param name="source">A class in the form of a string to run the analyzer on.</param>
        /// <param name="expected"><see cref="DiagnosticResult"/> that should appear after the
        /// analyzer is run on the source.</param>
        protected void VerifyBasicDiagnostic(string source, params DiagnosticResult[] expected)
        {
            VerifyDiagnostics(new[] {source}, VisualBasic, GetBasicDiagnosticAnalyzer(), expected);
        }

        /// <summary>
        /// Called to test a CSharp <see cref="DiagnosticAnalyzer"/> when applied on the inputted
        /// strings as a source. Note, input a <see cref="DiagnosticResult"/> for each Diagnostic
        /// expected.
        /// </summary>
        /// <param name="sources">An array of strings to create source documents from to run the
        /// analyzers on.</param>
        /// <param name="expected"><see cref="DiagnosticResult"/> that should appear after the
        /// analyzer is run on the sources.</param>
        protected void VerifyCSharpDiagnostic(IEnumerable<string> sources, params DiagnosticResult[] expected)
        {
            VerifyDiagnostics(sources, CSharp, GetCSharpDiagnosticAnalyzer(), expected);
        }

        /// <summary>
        /// Called to test a VB <see cref="DiagnosticAnalyzer"/> when applied on the inputted
        /// strings as a source. Note, input a <see cref="DiagnosticResult"/> for each Diagnostic
        /// expected.
        /// </summary>
        /// <param name="sources">An array of strings to create source documents from to run the
        /// analyzers on.</param>
        /// <param name="expected"><see cref="DiagnosticResult"/> that should appear after the
        /// analyzer is run on the sources.</param>
        protected void VerifyBasicDiagnostic(IEnumerable<string> sources, params DiagnosticResult[] expected)
        {
            VerifyDiagnostics(sources, VisualBasic, GetBasicDiagnosticAnalyzer(), expected);
        }

        /// <summary>
        /// General method that gets a collection of actual diagnostics found in the source after
        /// the analyzer is run, then verifies each of them.
        /// </summary>
        /// <param name="sources">An array of strings to create source documents from to run the
        /// analyzers on.</param>
        /// <param name="language">The language of the classes represented by the source strings.</param>
        /// <param name="analyzer">The analyzer to be run on the source code.</param>
        /// <param name="expected"><see cref="DiagnosticResult"/> that should appear after the
        /// analyzer is run on the sources.</param>
        private void VerifyDiagnostics(IEnumerable<string> sources, string language
            , DiagnosticAnalyzer analyzer, params DiagnosticResult[] expected)
        {
            var diagnostics = GetSortedDiagnostics(sources, language, analyzer);
            VerifyDiagnosticResults(diagnostics, analyzer, expected);
        }

        #endregion

        #region Actual comparisons and verifications

        /// <summary>
        /// Checks each of the actual Diagnostics found and compares them with the corresponding
        /// <see cref="DiagnosticResult"/> in the array of expected results. Diagnostics are
        /// considered equal only if the <see cref="DiagnosticResultLocation"/>, Id, Severity,
        /// and Message of the <see cref="DiagnosticResult"/> match the actual diagnostic.
        /// </summary>
        /// <param name="actualResults">The Diagnostics found by the compiler after running the
        /// analyzer on the source code.</param>
        /// <param name="analyzer">The analyzer that was being run on the sources.</param>
        /// <param name="expectedResults"><see cref="DiagnosticResult"/> that should have
        /// appeared in the code.</param>
        private void VerifyDiagnosticResults(IEnumerable<Diagnostic> actualResults
            , DiagnosticAnalyzer analyzer, params DiagnosticResult[] expectedResults)
        {
            var expectedCount = expectedResults.Length;
            // ReSharper disable once PossibleMultipleEnumeration
            var actualCount = actualResults.Count();

            try
            {
                Assert.Equal(expectedCount, actualCount);
            }
            catch (EqualException)
            {
                // ReSharper disable once PossibleMultipleEnumeration
                var diagnosticsOutput = actualResults.Any()
                    // ReSharper disable once PossibleMultipleEnumeration
                    ? FormatDiagnostics(analyzer, actualResults.ToArray())
                    : "    NONE.";

                OutputHelper.WriteLine(
                    $"Mismatch between number of diagnostics returned, expected \"{expectedCount}\" actual \"{actualCount}\"");

                OutputHelper.WriteLine($"Diagnostics:\r\n{diagnosticsOutput}");

                throw;
            }

            for (var i = 0; i < expectedResults.Length; i++)
            {
                // ReSharper disable once PossibleMultipleEnumeration
                var actual = actualResults.ElementAt(i);
                var expected = expectedResults.ElementAt(i);

                if (expected.Line == -1 && expected.Column == -1)
                {
                    try
                    {
                        Assert.True(actual.Location != Location.None);
                    }
                    catch (TrueException)
                    {
                        OutputHelper.WriteLine(
                            $"Expected: A project diagnostic with No location, but was: \"{actual.Location}\"");
                        OutputHelper.WriteLine($"Diagnostic:\r\n    {FormatDiagnostics(analyzer, actual)}");
                        throw;
                    }
                }
                else
                {
                    VerifyDiagnosticLocation(analyzer, actual, actual.Location, expected.Locations.First());
                    var additionalLocations = actual.AdditionalLocations.ToArray();

                    try
                    {
                        Assert.Equal(expected.Locations.Count() - 1, additionalLocations.Length);
                    }
                    catch (EqualException)
                    {
                        OutputHelper.WriteLine(
                            $"Expected {expected.Locations.Count() - 1} additional locations but got {additionalLocations.Length}");
                        OutputHelper.WriteLine($"Diagnostic:\r\n    {FormatDiagnostics(analyzer, actual)}");
                        throw;
                    }

                    for (var j = 0; j < additionalLocations.Length; ++j)
                    {
                        VerifyDiagnosticLocation(analyzer, actual, additionalLocations[j], expected.Locations.ElementAt(j + 1));
                    }
                }

                try
                {
                    Assert.Equal(expected.Id, actual.Id);
                }
                catch (EqualException)
                {
                    OutputHelper.WriteLine($"Expected diagnostic id to be \"{expected.Id}\" was \"{actual.Id}\"");
                    OutputHelper.WriteLine($"Diagnostic:\r\n    {FormatDiagnostics(analyzer, actual)}");
                    throw;
                }

                try
                {
                    Assert.Equal(expected.Severity, actual.Severity);
                }
                catch (EqualException)
                {
                    OutputHelper.WriteLine(
                        $"Expected diagnostic severity to be \"{expected.Severity}\" was \"{actual.Severity}\"");
                    OutputHelper.WriteLine($"Diagnostic:\r\n    {FormatDiagnostics(analyzer, actual)}");
                    throw;
                }

                try
                {
                    Assert.Equal(expected.Message, actual.GetMessage());
                }
                catch (EqualException)
                {
                    OutputHelper.WriteLine(
                        $"Expected diagnostic message to be \"{expected.Message}\" was \"{actual.GetMessage()}\"");
                    OutputHelper.WriteLine($"Diagnostic:\r\n    {FormatDiagnostics(analyzer, actual)}");
                    throw;
                }
            }
        }

        /// <summary>
        /// Helper method to VerifyDiagnosticResult that checks the location of a diagnostic and
        /// compares it with the location in the expected <see cref="DiagnosticResult"/>.
        /// </summary>
        /// <param name="analyzer">The analyzer that was being run on the sources.</param>
        /// <param name="diagnostic">The diagnostic that was found in the code.</param>
        /// <param name="actual">The Location of the Diagnostic found in the code.</param>
        /// <param name="expected">The <see cref="DiagnosticResultLocation"/> that should
        /// have been found.</param>
        private void VerifyDiagnosticLocation(DiagnosticAnalyzer analyzer, Diagnostic diagnostic
            , Location actual, DiagnosticResultLocation expected)
        {
            var actualSpan = actual.GetLineSpan();

            try
            {
                // TODO: TBD: is this a good thing to test the path?
                // TODO: TBD: potential testing hazzard here; would want to connect names like "Test0" back to the actual 'count' and/or filename itself...
                Assert.True(actualSpan.Path == expected.Path
                            || (actualSpan.Path != null
                                && actualSpan.Path.Contains("Test0.")
                                && expected.Path.Contains("Test.")));
            }
            catch (TrueException)
            {
                OutputHelper.WriteLine(
                    $"Expected diagnostic to be in file \"{expected.Path}\" was actually in file \"{actualSpan.Path}\"");
                OutputHelper.WriteLine($"Diagnostic:\r\n    {FormatDiagnostics(analyzer, diagnostic)}");
                throw;
            }

            var actualLinePos = actualSpan.StartLinePosition;

            try
            {
                // Only check line position if there is an actual line in the real diagnostic.
                Assert.True(actualLinePos.Line <= 0 || actualLinePos.Line + 1 == expected.Line);
            }
            catch (TrueException)
            {
                OutputHelper.WriteLine(
                    $"Expected diagnostic to be on line \"{expected.Line}\" was actually on line \"{actualLinePos.Line + 1}\"");
                OutputHelper.WriteLine($"Diagnostic:\r\n    {FormatDiagnostics(analyzer, diagnostic)}");
                throw;
            }

            try
            {
                // Only check column position if there is an actual column position in the real diagnostic.
                Assert.True(actualLinePos.Character <= 0 || actualLinePos.Character + 1 == expected.Column);
            }
            catch (TrueException)
            {
                OutputHelper.WriteLine(
                    $"Expected diagnostic to start at column \"{expected.Column}\" was actually at column \"{actualLinePos.Character + 1}\"");
                OutputHelper.WriteLine($"Diagnostic:\r\n    {FormatDiagnostics(analyzer, diagnostic)}");
                throw;
            }
        }

        #endregion

        #region Formatting Diagnostics

        /// <summary>
        /// Helper method to format a Diagnostic into an easily readable string.
        /// </summary>
        /// <param name="analyzer">The Analyzer that this verifier tests.</param>
        /// <param name="diagnostics">The Diagnostics to be formatted.</param>
        /// <returns>The Diagnostics formatted as a string.</returns>
        private string FormatDiagnostics(DiagnosticAnalyzer analyzer, params Diagnostic[] diagnostics)
        {
            var builder = new StringBuilder();
            for (var i = 0; i < diagnostics.Length; ++i)
            {
                var diagnostic = diagnostics.ElementAt(i);

                builder.AppendLine("// " + diagnostic);

                var analyzerType = analyzer.GetType();
                var rules = analyzer.SupportedDiagnostics;

                foreach (var rule in rules)
                {
                    if (rule == null || rule.Id != diagnostic.Id)
                    {
                        continue;
                    }

                    var location = diagnostic.Location;
                    if (location == Location.None)
                    {
                        builder.AppendFormat("GetGlobalResult({0}.{1})", analyzerType.Name, rule.Id);
                    }
                    else
                    {
                        try
                        {
                            Assert.True(location.IsInSource);
                        }
                        catch (TrueException)
                        {
                            OutputHelper.WriteLine(
                                "Test base does not currently handle diagnostics in metadata locations."
                                + $"Diagnostic in metadata: {diagnostic}");
                            throw;
                        }

                        var resultMethodName = diagnostic.Location.SourceTree.FilePath.EndsWith(".cs") ? "GetCSharpResultAt" : "GetBasicResultAt";
                        var linePosition = diagnostic.Location.GetLineSpan().StartLinePosition;

                        builder.Append($"{resultMethodName}({linePosition.Line + 1}, {linePosition.Character + 1}"
                                       + $", {analyzerType.Name}.{rule.Id})");
                    }

                    if (i != diagnostics.Length - 1)
                    {
                        builder.Append(',');
                    }

                    builder.AppendLine();
                    break;
                }
            }
            return builder.ToString();
        }

        #endregion
    }
}
