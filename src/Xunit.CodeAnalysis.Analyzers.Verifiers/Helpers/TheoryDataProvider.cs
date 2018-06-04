using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Xunit.CodeAnalysis
{
    /// <summary>
    /// Provides data sets for theory tests. Extend this class to have strongly-typed dataset
    /// provider method: <see cref="GetDataSets"/>.
    /// </summary>
    /// <inheritdoc />
    public abstract class TheoryDataProvider : IEnumerable<object[]>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public abstract IEnumerable<ITheoryDatum> GetDataSets();

        /// <summary>
        /// Returns the <see cref="IEnumerable{T}"/> for the Theory Data.
        /// </summary>
        /// <returns></returns>
        /// <inheritdoc />
        public IEnumerator<object[]> GetEnumerator() => GetDataSets().Select(x => x.ToParameterArray()).GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
