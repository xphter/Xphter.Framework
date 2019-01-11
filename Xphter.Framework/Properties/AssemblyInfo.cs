using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Web;
using Xphter.Framework.Web;

// General Information about an assembly is controlled through the following 
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
[assembly: AssemblyTitle("XphteR Framework")]
[assembly: AssemblyDescription("XphteR Framework")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("xphter.com")]
[assembly: AssemblyProduct("XphteR Framework")]
[assembly: AssemblyCopyright("copyright © xphter.com 2013-2018. all rights reserved.")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

// Setting ComVisible to false makes the types in this assembly not visible 
// to COM components.  If you need to access a type in this assembly from 
// COM, set the ComVisible attribute to true on that type.
[assembly: ComVisible(false)]

// The following GUID is for the ID of the typelib if this project is exposed to COM
[assembly: Guid("5c7d7b39-230c-42cf-bf9f-ef4b79dd0aa3")]

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
[assembly: AssemblyVersion("1.0.0.0")]
[assembly: AssemblyFileVersion("1.0.0.0")]
[assembly: PreApplicationStartMethod(typeof(CopyrightHttpModule), "Register")]
[assembly: InternalsVisibleTo("Xphter.Framework.Test, PublicKey=0024000004800000940000000602000000240000525341310004000001000100c72487088a73cd276078657cfb9f01aa2d4821cf3be241b050a98cf6b26383ca867bbb7233e2cd7eec618875cd341d2569dbbcb301e478d74f7fe0bd6f9831c9938c8115e3f192dc0c1bc92971c5bc9c1fe91f1112259f0999d9e730a09f9d0bad629d6438c633e779de28ca9c358604bb4875c0db85bdbd0b90c4eab9976ead")]
