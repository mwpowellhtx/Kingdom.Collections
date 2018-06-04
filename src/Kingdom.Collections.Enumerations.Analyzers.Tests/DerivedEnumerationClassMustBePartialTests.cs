using System;
using System.Collections.Generic;
using System.Linq;
using Xunit.CodeAnalysis;

namespace Kingdom.Collections
{
    using Xunit;
    using Xunit.Abstractions;
    using static Descriptors;
    using static DiagnosticVerifier;
    using static String;

    public class DerivedEnumerationClassMustBePartialTests
        : IClassFixture<DerivedEnumerationClassMustBePartialCodeFixVerifier>
    {
        private ITestOutputHelper OutputHelper { get; }

        private CodeFixVerifier Verifier { get; }

        // ReSharper disable once SuggestBaseTypeForParameter
        public DerivedEnumerationClassMustBePartialTests(ITestOutputHelper outputHelper
            , DerivedEnumerationClassMustBePartialCodeFixVerifier verifier)
        {
            Verifier = verifier;
            Verifier.OutputHelper = OutputHelper = outputHelper;
        }

        //    /// <summary>
        //    /// No diagnostics expected to show up.
        //    /// </summary>
        //    [Fact]
        //    public void TestMethod1()
        //    {
        //        var test = @"";

        //        Verifier.VerifyCSharpDiagnostic(test);
        //    }

        //    /// <summary>
        //    /// Diagnostic and CodeFix both triggered and checked for.
        //    /// </summary>
        //    [Fact]
        //    public void TestMethod2()
        //    {
        //        var test = @"
        //using System;
        //using System.Collections.Generic;
        //using System.Linq;
        //using System.Text;
        //using System.Threading.Tasks;
        //using System.Diagnostics;

        //namespace ConsoleApplication1
        //{
        //    class TypeName
        //    {   
        //    }
        //}";
        //        var expected = new DiagnosticResult
        //        {
        //            Id = "KingdomCollectionsEnumerationsAnalyzers",
        //            Message = string.Format("Type name '{0}' contains lowercase letters", "TypeName"),
        //            Severity = DiagnosticSeverity.Warning,
        //            Locations =
        //                new[]
        //                {
        //                    new DiagnosticResultLocation("Test0.cs", 11, 15)
        //                }
        //        };

        //        Verifier.VerifyCSharpDiagnostic(test, expected);

        //        var fixtest = @"
        //using System;
        //using System.Collections.Generic;
        //using System.Linq;
        //using System.Text;
        //using System.Threading.Tasks;
        //using System.Diagnostics;

        //namespace ConsoleApplication1
        //{
        //    class TYPENAME
        //    {   
        //    }
        //}";
        //        Verifier.VerifyCSharpFix(test, fixtest);
        //    }

        /// <summary>
        /// Diagnostic and Code Fix both triggered and checked.
        /// </summary>
        /// <param name="description"></param>
        /// <param name="givenSource"></param>
        /// <param name="diagnosticResults"></param>
        /// <param name="fixedSource"></param>
        [Theory, ClassData(typeof(TestCases))]
        public void Given_Source_then_Verify_Diagnostics_and_Code_Fixes(string description
            , string givenSource, string fixedSource, IEnumerable<DiagnosticResult> diagnosticResults)
        {
            Assert.NotNull(description);
            Assert.NotEmpty(description);

            OutputHelper.WriteLine($"Verifying diagnostics for \"{description}\".");

            Verifier.VerifyCSharpDiagnostic(givenSource, diagnosticResults.EnsureAtLeastEmpty().ToArray());

            Verifier.VerifyCSharpFix(givenSource, fixedSource);
        }

        // TODO: TBD: borrowing from the concepts employed by the Record Generator project... however...
        // TODO: TBD we could potentially simplify this from a "data provider" to a simple "member" data source...
        private class TestCases : TheoryDataProvider
        {
            private static IEnumerable<ITheoryDatum> PrivateDataSets { get; }

            static TestCases()
            {
                IEnumerable<ITheoryDatum> GetPrivateDataSets()
                {
                    var enumerationTypeFullName = typeof(Enumeration).FullName;
                    var flagsEnumerationTypeFullName = typeof(FlagsEnumerationAttribute).FullName;

                    var fileName = $"{DefaultFilePathPrefix}.{CSharpDefaultFileExt}";

                    yield return new GeneratorTheoryData
                    {
                        Description = "Empty source",
                        GivenSource = Empty
                    };

                    {
                        const string g = "public class TestClass {}";
                        yield return new GeneratorTheoryData
                        {
                            Description = $"ignore non-{enumerationTypeFullName} classes",
                            GivenSource = g,
                            FixedSource = g
                        };
                    }

                    {
                        /* Whereas the inspiration behind this approach, Record Generator, was
                         doing something like "crop raw indent", I think we just leverage the
                         built in language features and enter it as such. If we cannot deal with
                         that, so be it.
                         https://github.com/amis92/RecordGenerator/blob/f761bcf58a894bcc15233dbb465747a21c6da53c/test/Amadevus.RecordGenerator.Analyzers.Test/Helpers/Extensions.cs#L14 */
                        const string g = @"using Kingdom.Collections;

namespace MyClasses
{
    public class TestClass : Enumeration<TestClass>
    {
    }
}";

                        yield return new GeneratorTheoryData
                        {
                            Description = $"ignore undecorated {enumerationTypeFullName} class",
                            GivenSource = g,
                            FixedSource = g
                        };
                    }

                    {
                        const string g = @"using Kingdom.Collections;

namespace MyClasses
{
    [FlagsEnumeration]
    public partial class TestClass : Enumeration<TestClass>
    {
    }
}";
                        yield return new GeneratorTheoryData
                        {
                            Description = $"nothing to do for partial [{flagsEnumerationTypeFullName}] {enumerationTypeFullName} class",
                            GivenSource = g,
                            FixedSource = g
                        };
                    }

                    {
                        /* The extrinsic comments and spacing here are VERY intentional and serve
                         to illustrate where we expect the diagnostic to occur in relation to the
                         provided source code. */

                        const int line = 5;
                        const int character = 17;
                        const string g = @"using Kingdom.Collections;

namespace MyClasses
{
    [FlagsEnumeration]
    public class TestClass : Enumeration<TestClass>" /*
                 ^ we expect error requiring correction precisely here,
                  literally line 6 (5+1) character 18 (17+1) */
                                         + @"
    {
    }
}";
                        const string f = @"using Kingdom.Collections;

namespace MyClasses
{
    [FlagsEnumeration]
    public partial class TestClass : Enumeration<TestClass>
    {
    }
}";
                        yield return new GeneratorTheoryData
                        {
                            Description = $"transform non-partial [{flagsEnumerationTypeFullName}] {enumerationTypeFullName} class",
                            GivenSource = g,
                            FixedSource = f,
                            ExpectedDiagnostics = DiagnosticResult.Create(X1000_DerivedEnumerationMustBePartial
                                , DiagnosticResultLocation.Create(fileName, line + 1, character + 1)).ToArrayArray()
                        };
                    }
                }

                PrivateDataSets = GetPrivateDataSets().ToArray();
            }

            public override IEnumerable<ITheoryDatum> GetDataSets() => PrivateDataSets;
        }

        public class GeneratorTheoryData : ITheoryDatum
        {
            public string Description { get; set; }

            public string GivenSource { get; set; }

            public IEnumerable<DiagnosticResult> ExpectedDiagnostics { get; set; }

            public string FixedSource { get; set; }

            /// <summary>
            /// Returns the parameters in test method argument order.
            /// </summary>
            /// <returns></returns>
            /// <see cref="Given_Source_then_Verify_Diagnostics_and_Code_Fixes"/>
            private IEnumerable<object> GetParameters()
            {
                // Mind the ordering here, must be aligned with the test method.
                yield return Description;
                yield return GivenSource;
                yield return FixedSource;
                yield return ExpectedDiagnostics;
            }

            public object[] ToParameterArray() => GetParameters().ToArray();
        }
    }
}
