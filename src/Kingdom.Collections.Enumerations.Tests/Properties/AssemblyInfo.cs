using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

// General Information about an assembly is controlled through the following 
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
[assembly: AssemblyTitle("Kingdom.Collections.Enumerations.Tests")]
[assembly: AssemblyDescription("")]

// Setting ComVisible to false makes the types in this assembly not visible 
// to COM components.  If you need to access a type in this assembly from 
// COM, set the ComVisible attribute to true on that type.
[assembly: ComVisible(false)]

// The following GUID is for the ID of the typelib if this project is exposed to COM
[assembly: Guid("3557d09f-2fed-4535-a974-7a26042d3174")]

// Version information for an assembly consists of the following four values:
//
//      Major Version
//      Minor Version 
//      Build Number
//      Revision
//
// You can specify all the values or you can default the Build and Revision Numbers 
// by using the '*' as shown below:
[assembly: AssemblyVersion("1.1.4.16")]
[assembly: AssemblyFileVersion("1.1.4.16")]

/* Yes, this is not a typo. We will be publishing the test framework, which itself
 requires a bit of verification in order to test that our assumptions are valid. */
[assembly: InternalsVisibleTo("Kingdom.Collections.Enumerations.Tests.Tests")]
