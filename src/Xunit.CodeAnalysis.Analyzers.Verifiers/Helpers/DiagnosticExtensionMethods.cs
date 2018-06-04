using System.Collections.Generic;
using System.Linq;

namespace Xunit.CodeAnalysis
{
    using Microsoft.CodeAnalysis;

    internal static class DiagnosticExtensionMethods
    {
        public static IEnumerable<Diagnostic> SortDiagnostics(this IEnumerable<Diagnostic> diagnostics)
            => diagnostics.OrderBy(d => d.Location.SourceSpan.Start);
    }
}
