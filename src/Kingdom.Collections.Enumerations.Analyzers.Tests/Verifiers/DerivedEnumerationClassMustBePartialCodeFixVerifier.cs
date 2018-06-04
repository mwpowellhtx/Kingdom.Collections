using System.Runtime.Serialization;
using Microsoft.CodeAnalysis;

namespace Kingdom.Collections
{
    using Microsoft.CodeAnalysis.CodeFixes;
    using Microsoft.CodeAnalysis.Diagnostics;
    //using Xunit.Abstractions;
    using Xunit.CodeAnalysis;

    public class DerivedEnumerationClassMustBePartialCodeFixVerifier : CodeFixVerifier
    {
        //// TODO: TBD: eventually, I'd like to see Xunit resolve the cross cutting concern for fixtures, but I do not thing that is happening ATM...
        //public DerivedEnumerationClassMustBePartialCodeFixVerifier(ITestOutputHelper outputHelper)
        //    : base(outputHelper)
        //{
        //}

        protected override CodeFixProvider GetCSharpCodeFixProvider()
            => new DerivedEnumerationClassMustBePartialCodeFixProvider();

        protected override DiagnosticAnalyzer GetCSharpDiagnosticAnalyzer()
            => new DerivedEnumerationClassMustBePartial();

        #region We must also reference bits from the deliverable assemblies

        private static readonly MetadataReference SystemRuntimeReference
            = MetadataReference.CreateFromFile(typeof(ISerializable).Assembly.Location);

        private static readonly MetadataReference ImmutableBitArrayReference
            = MetadataReference.CreateFromFile(typeof(ImmutableBitArray).Assembly.Location);

        private static readonly MetadataReference EnumerationsReference
            = MetadataReference.CreateFromFile(typeof(Enumeration).Assembly.Location);

        private static readonly MetadataReference FlagsEnumerationAttributeReference
            = MetadataReference.CreateFromFile(typeof(FlagsEnumerationAttribute).Assembly.Location);

        #endregion

        /// <summary>
        /// It seems as though Metadata Reference order is a thing. So ensure that we are
        /// referencing the important bits from the base class first, following the more
        /// application specific bits afterward.
        /// </summary>
        /// <param name="sln"></param>
        /// <param name="projectId"></param>
        /// <returns></returns>
        /// <inheritdoc />
        /// <see cref="SystemRuntimeReference"/>
        /// <see cref="ImmutableBitArrayReference"/>
        /// <see cref="EnumerationsReference"/>
        /// <see cref="FlagsEnumerationAttributeReference"/>
        protected override Solution AddProjectReferences(Solution sln, ProjectId projectId)
        {
            return base.AddProjectReferences(sln, projectId)
                .AddMetadataReference(projectId, SystemRuntimeReference)
                .AddMetadataReference(projectId, ImmutableBitArrayReference)
                .AddMetadataReference(projectId, EnumerationsReference)
                .AddMetadataReference(projectId, FlagsEnumerationAttributeReference)
                ;
        }
    }
}
