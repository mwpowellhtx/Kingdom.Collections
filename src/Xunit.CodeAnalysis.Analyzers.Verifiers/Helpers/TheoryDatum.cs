using System.Collections.Generic;
using System.Linq;

namespace Xunit.CodeAnalysis
{
    /// <summary>
    /// Provides factory method <see cref="Create{TSystemUnderTest,TExecptedOutput}"/>.
    /// </summary>
    /// <inheritdoc />
    public abstract class TheoryDatum : ITheoryDatum
    {
        internal static ITheoryDatum Create<TSystemUnderTest, TExecptedOutput>(TSystemUnderTest systemUnderTest,
            TExecptedOutput expectedOutput, string description)
            => new TheoryDatum<TSystemUnderTest, TExecptedOutput>
            {
                SystemUnderTest = systemUnderTest,
                Description = description,
                ExpectedOutput = expectedOutput
            };

        /// <summary>
        /// Returns the Theory Datum Parameters.
        /// </summary>
        /// <returns></returns>
        /// <inheritdoc />
        public abstract object[] ToParameterArray();
    }

    /// <summary>
    /// Type-parametrized data set for theory tests, used by
    /// <see cref="TheoryDatum.Create{TSystemUnderTest,TExecptedOutput}"/>.
    /// </summary>
    /// <typeparam name="TSystemUnderTest"></typeparam>
    /// <typeparam name="TExecptedOutput"></typeparam>
    /// <inheritdoc cref="TheoryDatum" />
    public class TheoryDatum<TSystemUnderTest, TExecptedOutput> : TheoryDatum
    {
        /// <summary>
        /// Gets or sets the SystemUnderTest.
        /// </summary>
        public TSystemUnderTest SystemUnderTest { get; set; }

        /// <summary>
        /// Gets or sets the Description.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the ExpectedOutput.
        /// </summary>
        public TExecptedOutput ExpectedOutput { get; set; }

        private IEnumerable<object> GetParameters()
        {
            yield return SystemUnderTest;
            yield return ExpectedOutput;
            yield return Description;
        }

        /// <summary>
        /// Returns the Theory Datum Parameters.
        /// </summary>
        /// <returns></returns>
        /// <see cref="GetParameters"/>
        /// <inheritdoc />
        public override object[] ToParameterArray() => GetParameters().ToArray();
    }
}
