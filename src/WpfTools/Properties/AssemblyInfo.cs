using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Markup;

// General Information about an assembly is controlled through the following
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
[assembly: AssemblyTitle("WpfTools")]
[assembly: AssemblyDescription("An auxiliary set of components for developing WPF applications")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("Alexey Pozdnyakov (Algel)")]
[assembly: AssemblyProduct("WpfTools")]
[assembly: AssemblyCopyright("Copyright © Alexey Pozdnyakov 2018")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

// Setting ComVisible to false makes the types in this assembly not visible
// to COM components.  If you need to access a type in this assembly from
// COM, set the ComVisible attribute to true on that type.
[assembly: ComVisible(false)]

// The following GUID is for the ID of the typelib if this project is exposed to COM
[assembly: Guid("cd733cb1-97bf-40d4-bc3f-638c7e190857")]


[assembly: ThemeInfo(
    ResourceDictionaryLocation.None, //where theme specific resource dictionaries are located
    //(used if a resource is not found in the page, 
    // or application resource dictionaries)
    ResourceDictionaryLocation.SourceAssembly //where the generic resource dictionary is located
    //(used if a resource is not found in the page, 
    // app, or any theme specific resource dictionaries)
)]

[assembly: XmlnsPrefix("https://github.com/algel/WpfTools", "awt")]
[assembly: XmlnsDefinition("https://github.com/algel/WpfTools", "Algel.WpfTools.Windows.Controls")]
[assembly: XmlnsDefinition("https://github.com/algel/WpfTools", "Algel.WpfTools.Windows.Data")]
[assembly: XmlnsDefinition("https://github.com/algel/WpfTools", "Algel.WpfTools.Windows.Markup")]

// Version information for an assembly consists of the following four values:
//
//      Major Version
//      Minor Version
//      Build Number
//      Revision
//
// You can specify all the values or you can default the Build and Revision Numbers
// by using the '*' as shown below:
// [assembly: AssemblyVersion("1.0.*")]
[assembly: AssemblyVersion("2018.5.10.0730")]
[assembly: AssemblyFileVersion("2018.5.10.0730")]
