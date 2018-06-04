using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Xunit.CodeAnalysis
{
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CodeActions;
    using Microsoft.CodeAnalysis.Formatting;
    using Microsoft.CodeAnalysis.Simplification;

    /// <summary>
    /// Parent class of all Unit tests made for diagnostics with code fixes. Contains methods
    /// used to verify correctness of code fixes. Diagnostic Producer class with extra methods
    /// dealing with applying code fixes. Refactored from original project template as services
    /// passed to the test class.
    /// </summary>
    /// <inheritdoc />
    public partial class CodeFixVerifier
    {
        // ReSharper disable once SuggestBaseTypeForParameter
        /// <summary>
        /// Apply the inputted Code Action to the inputted document. Meant to be used to apply
        /// code fixes.
        /// </summary>
        /// <param name="document">The Document to apply the fix on.</param>
        /// <param name="codeAction">A Code Action that will be applied to the Document.</param>
        /// <returns>A Document with the changes from the CodeAction.</returns>
        private static Document ApplyFix(Document document, CodeAction codeAction)
        {
            var ops = codeAction.GetOperationsAsync(CancellationToken.None).Result;
            var sln = ops.OfType<ApplyChangesOperation>().Single().ChangedSolution;
            return sln.GetDocument(document.Id);
        }

        /// <summary>
        /// Compare two collections of Diagnostics,and return a list of any new diagnostics
        /// that appear only in the second collection. Note, considers Diagnostics to be the
        /// same if they have the same Ids.  In the case of multiple diagnostics with the same
        /// Id in a row, this method may not necessarily return the new one.
        /// </summary>
        /// <param name="diagnostics">The Diagnostics that existed in the code before the
        /// Code Fix was applied.</param>
        /// <param name="newDiagnostics">The Diagnostics that exist in the code after the
        /// Code Fix was applied.</param>
        /// <returns>A list of Diagnostics that only surfaced in the code after the Code Fix
        /// was applied</returns>
        private static IEnumerable<Diagnostic> GetNewDiagnostics(IEnumerable<Diagnostic> diagnostics
            , IEnumerable<Diagnostic> newDiagnostics)
        {
            var oldArray = diagnostics.SortDiagnostics().ToArray();
            var newArray = newDiagnostics.SortDiagnostics().ToArray();

            for (int oldIndex = 0, newIndex = 0; newIndex < newArray.Length; newIndex++)
            {
                if (oldIndex < oldArray.Length && oldArray[oldIndex].Id == newArray[newIndex].Id)
                {
                    ++oldIndex;
                }
                else
                {
                    yield return newArray[newIndex];
                }
            }
        }

        //private static IEnumerable<Diagnostic> GetNewDiagnostics(IEnumerable<Diagnostic> diagnostics
        //    , IEnumerable<Diagnostic> newDiagnostics)
        //{
        //    // ReSharper disable once PossibleMultipleEnumeration
        //    var oldIds = diagnostics.Select(d => d.Id).OrderBy(x => x).ToArray();
        //    // ReSharper disable once PossibleMultipleEnumeration
        //    var newIds = newDiagnostics.Select(d => d.Id).OrderBy(x => x).ToArray();

        //    // TODO: TBD: okay, so the "Id" in this instance is the actual compiler diagnostic error/warning, etc, I think...
        //    // TODO: TBD: which, there may be numerous, and duplicate, just besides...
        //    // ReSharper disable once PossibleMultipleEnumeration
        //    var newDiagnosticsDictionary = newDiagnostics.ToDictionary(x => x.Id, x => x);

        //    foreach (var i in newIds.Except(oldIds))
        //    {
        //        yield return newDiagnosticsDictionary[i];
        //    }
        //}

        /// <summary>
        /// Get the existing compiler diagnostics on the inputted document.
        /// </summary>
        /// <param name="document">The Document to run the compiler diagnostic analyzers on.</param>
        /// <returns>The compiler diagnostics that were found in the code.</returns>
        private static IEnumerable<Diagnostic> GetCompilerDiagnostics(Document document)
            => document.GetSemanticModelAsync().Result.GetDiagnostics();

        /// <summary>
        /// Given a document, turn it into a string based on the syntax root.
        /// </summary>
        /// <param name="document">The Document to be converted to a string.</param>
        /// <returns>A string containing the syntax of the Document after formatting.</returns>
        private static string GetStringFromDocument(Document document)
        {
            var sdoc = Simplifier.ReduceAsync(document, Simplifier.Annotation).Result;
            var root = sdoc.GetSyntaxRootAsync().Result;
            root = Formatter.Format(root, Formatter.Annotation, sdoc.Project.Solution.Workspace);
            return root.GetText().ToString();
        }
    }
}

