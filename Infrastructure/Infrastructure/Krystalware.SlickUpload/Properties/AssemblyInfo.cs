using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Web.UI;
using System;
using System.Security;
using System.Security.Permissions;

// General Information about an assembly is controlled through the following 
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
[assembly: AssemblyTitle("SlickUpload")]
[assembly: SecurityRules(SecurityRuleSet.Level1)]
// TODO: make this better
[assembly: AssemblyDescription("ASP.NET component that provides advanced HTTP uploading support.")]
//[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("Krystalware")]
[assembly: AssemblyProduct("SlickUpload")]
[assembly: AssemblyCopyright("Copyright © 2011 Krystalware. All Rights Reserved.")]
//[assembly: AssemblyTrademark("")]
//[assembly: AssemblyCulture("")]

// Version information for an assembly consists of the following four values:
//
//      Major Version
//      Minor Version 
//      Build Number
//      Revision
//
// You can specify all the values or you can default the Build and Revision Numbers 
// by using the '*' as shown below:
[assembly: AssemblyVersion("6.1.8.*")]
[assembly: AssemblyFileVersion("6.1.8.*")]

[assembly: WebResource("Krystalware.SlickUpload.Resources.SlickUpload.js", "text/javascript")]
[assembly: WebResource("Krystalware.SlickUpload.Resources.PoweredBy.png", "image/png")]

[assembly: ComVisible(false)]
[assembly: CLSCompliant(true)]

[assembly: AllowPartiallyTrustedCallers]

[assembly: SecurityPermission(SecurityAction.RequestMinimum, UnmanagedCode = true)]
[assembly: ReflectionPermission(SecurityAction.RequestMinimum, MemberAccess = true)]

//[assembly: LicensePublicKey("<RSAKeyValue><Modulus>tk384UItx23mrOJcdDqipI05NXJKZAbUL0ewFaloFzpUBFV8AQd1hUSZ9XVgNAPqBLej9rUz4v08vi2rQ/fqJXtWieQSdwa7fe+ZjaS2qYGeBWZVpEDXwT5fG63+c2MlLjX0fKMQ7FU0OkZPo76Sj1O5cIjMlX9nvOQnevW0ABM=</Modulus><Exponent>AQAB</Exponent></RSAKeyValue>")]

[assembly: Obfuscation(Feature = "disable-encrypt-resources")]
//[assembly: Obfuscation(Feature = "disable-control-flow")]
[assembly: Obfuscation(Feature = "disable-cleanup")]
//[assembly: Obfuscation(Feature = "disable-optimize")]
//[assembly: Obfuscation(Feature = "disable-anti-decompiler")]
//[assembly: Obfuscation(Feature=@"strong-name-key /file:..\..\..\..\..\..\..\..\Krystalware.snk")]

