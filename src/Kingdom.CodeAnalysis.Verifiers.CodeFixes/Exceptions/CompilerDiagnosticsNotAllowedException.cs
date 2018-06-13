using System;

namespace Kingdom.CodeAnalysis.Verifiers
{
    /// <summary>
    /// 
    /// </summary>
    /// <inheritdoc />
    public sealed class CompilerDiagnosticsNotAllowedException : Exception
    {
        internal CompilerDiagnosticsNotAllowedException(string message)
            : base(message)
        {
        }
    }
}
