using System;

namespace Kingdom.Collections
{
    /// <summary>
    /// This is a stand alone place holder for purposes of anticipating the XUnit migration path.
    /// </summary>
    [Obsolete("Placeholder anticipating xunit migration path")]
    public interface ITestOutputHelper
    {
        void WriteLine(string format, params object[] args);
    }
}
