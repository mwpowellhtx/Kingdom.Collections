//using System;
//using System.IO;

//// TODO: TBD: which, this implementation is no longer necessary...
//namespace Kingdom.Collections
//{
//    using static Console;

//    /// <summary>
//    /// This is a stand alone place holder for purposes of anticipating the XUnit migration path.
//    /// </summary>
//    /// <inheritdoc />
//    [Obsolete("Placeholder anticipating xunit migration path")]
//    public class TestOutputHelper : ITestOutputHelper
//    {
//        private TextWriter Writer { get; }

//        public TestOutputHelper()
//            : this(Out)
//        {
//        }

//        public TestOutputHelper(TextWriter writer)
//        {
//            Writer = writer;
//        }

//        public void WriteLine(string format, params object[] args) => Writer.WriteLine(format, args);
//    }
//}
