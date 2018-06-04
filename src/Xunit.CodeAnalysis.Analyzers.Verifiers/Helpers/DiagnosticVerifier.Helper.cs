using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;

namespace Xunit.CodeAnalysis
{
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.Diagnostics;
    using Microsoft.CodeAnalysis.Text;
    using static LanguageNames;

    /// <summary>
    /// Class for turning strings into documents and getting the diagnostics on them.
    /// Parent class of all Unit Tests for Diagnostic Analyzers.
    /// </summary>
    public abstract partial class DiagnosticVerifier
    {
        #region We must reference a couple of core bits useful and/or necessary for every Verifier usage

        private static readonly MetadataReference CorlibReference
            = MetadataReference.CreateFromFile(typeof(object).Assembly.Location);

        private static readonly MetadataReference SystemCoreReference
            = MetadataReference.CreateFromFile(typeof(Enumerable).Assembly.Location);

        private static readonly MetadataReference CSharpSymbolsReference
            = MetadataReference.CreateFromFile(typeof(CSharpCompilation).Assembly.Location);

        private static readonly MetadataReference CodeAnalysisReference
            = MetadataReference.CreateFromFile(typeof(Compilation).Assembly.Location);

        #endregion

        /// <summary>
        /// Test
        /// </summary>
        public const string DefaultFilePathPrefix = "Test";

        /// <summary>
        /// cs
        /// </summary>
        public const string CSharpDefaultFileExt = "cs";

        /// <summary>
        /// vb
        /// </summary>
        public const string VisualBasicDefaultExt = "vb";

        /// <summary>
        /// TestProject
        /// </summary>
        public const string TestProjectName = "TestProject";

        #region  Get Diagnostics

        /// <summary>
        /// Given classes in the form of strings, their language, and an IDiagnosticAnalyzer to apply to it, return the diagnostics found in the string after converting it to a document.
        /// </summary>
        /// <param name="sources">Classes in the form of strings</param>
        /// <param name="language">The language the source classes are in</param>
        /// <param name="analyzer">The analyzer to be run on the sources</param>
        /// <returns>An IEnumerable of Diagnostics that surfaced in the source code, sorted by Location</returns>
        private IEnumerable<Diagnostic> GetSortedDiagnostics(IEnumerable<string> sources
            , string language, DiagnosticAnalyzer analyzer)
            => GetSortedDiagnosticsFromDocuments(analyzer, GetDocuments(sources, language));

        /// <summary>
        /// Given an analyzer and a document to apply it to, run the analyzer and gather an array of diagnostics found in it.
        /// The returned diagnostics are then ordered by location in the source document.
        /// </summary>
        /// <param name="analyzer">The analyzer to run on the documents</param>
        /// <param name="documents">The Documents that the analyzer will be run on</param>
        /// <returns>An IEnumerable of Diagnostics that surfaced in the source code, sorted by Location</returns>
        protected static IEnumerable<Diagnostic> GetSortedDiagnosticsFromDocuments(DiagnosticAnalyzer analyzer
            , params Document[] documents)
            => GetDiagnosticsFromDocuments(analyzer, documents).SortDiagnostics();

        /// <summary>
        /// Given an analyzer and a document to apply it to, run the analyzer and gather an array of diagnostics found in it.
        /// The returned diagnostics are then ordered by location in the source document.
        /// </summary>
        /// <param name="analyzer">The analyzer to run on the documents</param>
        /// <param name="documents">The Documents that the analyzer will be run on</param>
        /// <returns>An IEnumerable of Diagnostics that surfaced in the source code, sorted by Location</returns>
        private static IEnumerable<Diagnostic> GetDiagnosticsFromDocuments(DiagnosticAnalyzer analyzer
            , params Document[] documents)
        {
            var projects = new HashSet<Project>();

            foreach (var document in documents)
            {
                projects.Add(document.Project);
            }

            foreach (var project in projects)
            {
                var compilationWithAnalyzers = project.GetCompilationAsync().Result
                    .WithAnalyzers(ImmutableArray.Create(analyzer));

                var diagnostics = compilationWithAnalyzers.GetAnalyzerDiagnosticsAsync().Result;

                foreach (var diagnostic in diagnostics)
                {
                    if (diagnostic.Location == Location.None || diagnostic.Location.IsInMetadata)
                    {
                        yield return diagnostic;
                    }
                    else
                    {
                        // TODO: TBD: and if the async/result does not get too blocked...
                        // TODO: TBD or perhaps there is some sort of observable pattern that could better be used there...
                        if (documents.Select(d => d.GetSyntaxTreeAsync().Result)
                            .Any(t => t == diagnostic.Location.SourceTree))
                        {
                            yield return diagnostic;
                        }
                    }
                }
            }
        }

        #endregion

        #region Set up compilation and documents

        /// <summary>
        /// Given an array of strings as sources and a language, turn them into a project and return the documents and spans of it.
        /// </summary>
        /// <param name="sources">Classes in the form of strings</param>
        /// <param name="language">The language the source code is in</param>
        /// <returns>A Tuple containing the Documents produced from the sources and their TextSpans if relevant</returns>
        private Document[] GetDocuments(IEnumerable<string> sources, string language)
        {
            var languages = new[] {CSharp, VisualBasic};

            if (languages.All(l => l != language))
            {
                throw new ArgumentException($"Unsupported language \"{language}\"");
            }

            // ReSharper disable once PossibleMultipleEnumeration
            var project = CreateProject(sources, language);
            var documents = project.Documents.ToArray();

            // ReSharper disable once PossibleMultipleEnumeration
            if (sources.Count() != documents.Length)
            {
                throw new InvalidOperationException("Amount of sources did not match amount of Documents created");
            }

            return documents;
        }

        /// <summary>
        /// Create a Document from a string through creating a project that contains it.
        /// </summary>
        /// <param name="source">Classes in the form of a string</param>
        /// <param name="language">The language the source code is in</param>
        /// <returns>A Document created from the source string</returns>
        protected Document CreateDocument(string source, string language = CSharp)
            => CreateProject(new[] {source}, language).Documents.First();

        /// <summary>
        /// Adds <see cref="ProjectId"/> references or other details. Override in order to
        /// reference additional bits over and above these.
        /// </summary>
        /// <param name="sln"></param>
        /// <param name="projectId"></param>
        /// <returns></returns>
        /// <see cref="CorlibReference"/>
        /// <see cref="SystemCoreReference"/>
        /// <see cref="CSharpSymbolsReference"/>
        /// <see cref="CodeAnalysisReference"/>
        protected virtual Solution AddProjectReferences(Solution sln, ProjectId projectId)
        {
            return sln.AddMetadataReference(projectId, CorlibReference)
                .AddMetadataReference(projectId, SystemCoreReference)
                .AddMetadataReference(projectId, CSharpSymbolsReference)
                .AddMetadataReference(projectId, CodeAnalysisReference)
                ;
        }

        /// <summary>
        /// Create a project using the inputted strings as sources.
        /// </summary>
        /// <param name="sources">Classes in the form of strings</param>
        /// <param name="language">The language the source code is in</param>
        /// <returns>A Project created out of the Documents created from the source strings</returns>
        private Project CreateProject(IEnumerable<string> sources, string language = CSharp)
        {
            var fileExt = language == CSharp ? CSharpDefaultFileExt : VisualBasicDefaultExt;

            var projectId = ProjectId.CreateNewId(TestProjectName);

            var sln = AddProjectReferences(new AdhocWorkspace().CurrentSolution
                    .AddProject(projectId, TestProjectName, TestProjectName, language)
                , projectId);

            var count = 0;

            foreach (var source in sources)
            {
                var newFileName = $"{DefaultFilePathPrefix}{count++}.{fileExt}";
                var documentId = DocumentId.CreateNewId(projectId, newFileName);
                sln = sln.AddDocument(documentId, newFileName, SourceText.From(source));
            }

            return sln.GetProject(projectId);
        }

        #endregion
    }
}

