namespace Xunit.CodeAnalysis
{
    /// <summary>
    /// Single set of arguments for a theory.
    /// </summary>
    /// <see cref="!:http://stackoverflow.com/questions/22093843/pass-complex-parameters-to-theory"/>
    public interface ITheoryDatum
    {
        /// <summary>
        /// Gets the Theory Datum as an <see cref="System.Object"/> array.
        /// </summary>
        /// <returns></returns>
        object[] ToParameterArray();
    }
}
