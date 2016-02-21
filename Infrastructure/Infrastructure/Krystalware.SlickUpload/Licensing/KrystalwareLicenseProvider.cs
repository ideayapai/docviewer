using System;
using System.ComponentModel;
using System.IO;
using System.Reflection;
using System.Security.Cryptography.Xml;
using System.Web.Hosting;
using System.Xml;

namespace Krystalware.SlickUpload.Licensing
{
[Obfuscation(Feature = "cleanup", Exclude = true, ApplyToMembers=true)]
#if LICENSEASSEMBLY
	public enum LicenseScope
#else
	internal enum LicenseScope
#endif
	{
        [Obfuscation(Feature = "rename", Exclude = true)]
        WebSite = 0,
        [Obfuscation(Feature = "rename", Exclude = true)]
        Server = 1,
        [Obfuscation(Feature = "rename", Exclude = true)]
        PhysicalSite = 2,
        [Obfuscation(Feature = "rename", Exclude = true)]
        Redistributable = 3,
        [Obfuscation(Feature = "rename", Exclude = true)]
        Enterprise = 4
	}

[Obfuscation(Feature = "cleanup", Exclude = true, ApplyToMembers = true)]
#if LICENSEASSEMBLY
	public enum LicenseType
#else
	internal enum LicenseType
#endif
	{
        [Obfuscation(Feature = "rename", Exclude = true)]
        Evaluation = 0,
        [Obfuscation(Feature = "rename", Exclude = true)]
        Commercial = 2,
        [Obfuscation(Feature = "rename", Exclude = true)]
        NonCommercial = 1
	}

	/// <summary>
	/// Summary description for KrystalwareLicenseProvider.
	/// </summary>
#if LICENSEASSEMBLY
	public sealed class KrystalwareLicenseProvider
#else
    internal sealed class KrystalwareLicenseProvider// : LicenseProvider
#endif
    {
        internal KrystalwareLicenseProvider()
        { }

		//internal License GetLicense(LicenseContext context, Type type, object instance, bool allowExceptions)
        internal static License GetLicense(Type type, string key)
		{
			//if (System.Security.SecurityManager.SecurityEnabled == false)
			//	return null;
				
			KrystalwareRuntimeLicense l = null;

			string assembly = type.Assembly.GetName().Name;
			string licenseFile = assembly + ".xml.lic";

			// First, check if someone has set the license manually
			PropertyInfo[] licenseProperties = type.GetProperties(BindingFlags.NonPublic | BindingFlags.Static);

			foreach (PropertyInfo pi in licenseProperties)
			{
				if (pi.PropertyType == typeof(KrystalwareRuntimeLicense))
				{
					l = pi.GetValue(null, null) as KrystalwareRuntimeLicense;

					break;
				}
			}
            
            // First, check to see if there's a license file with the app
            if (l == null)
                l = LoadLicenseFromFile(type, HostingEnvironment.MapPath("~/"));

            // Next, check to see if there's a license file in the bin folder
            if (l == null)
                l = LoadLicenseFromFile(type, HostingEnvironment.MapPath("~/bin/"));

            // Next, check to see if there's a license file in the App_Data folder
            if (l == null)
                l = LoadLicenseFromFile(type, HostingEnvironment.MapPath("~/App_Data/"));

            // Next, check to see if there's a license resource somewhere
			if (l == null)
				l = LoadLicenseFromResource(type);

			// Next, check to see if there's a license file with the assembly
			if (l == null)
				l = LoadLicenseFromFile(type);
			
            if (l != null)
			{
				//string key = ((LicensePublicKeyAttribute)Attribute.GetCustomAttribute(type.Assembly, typeof(LicensePublicKeyAttribute))).XmlString;

				if (key != null)
				{
					KeyInfo keyInfo = LicenseValidator.GetKeyInfoFromXml(key);

					LicenseValidator validator = new LicenseValidator(l);

					if (!validator.IsValid(keyInfo))
						l = null;
				}
				else
				{
					l = null;
				}
			}

			// If not, create a new evaluation license
			if (l == null)
			{
				string version = type.Assembly.GetName().Version.Major.ToString();

				l = new KrystalwareRuntimeLicense(assembly, version);
			}

			return l;
		}

#if LICENSEASSEMBLY
		public static KrystalwareRuntimeLicense LoadLicenseFromResource(Type t)
#else
		internal static KrystalwareRuntimeLicense LoadLicenseFromResource(Type t)
#endif
		{
            try
            {
                string assembly = t.Assembly.GetName().Name;
                string licenseFile = assembly + ".xml.lic";

                return LoadLicenseFromResource(assembly + "." + licenseFile);
            }
            catch
            {
                return null;
            }
		}

#if LICENSEASSEMBLY
		public static KrystalwareRuntimeLicense LoadLicenseFromResource(string resource)
#else
		internal static KrystalwareRuntimeLicense LoadLicenseFromResource(string resource)
#endif
		{
            try
            {
                foreach (Assembly a in AppDomain.CurrentDomain.GetAssemblies())
                {

                    if (!(a is System.Reflection.Emit.AssemblyBuilder) &&
                              a.GetType().FullName != "System.Reflection.Emit.InternalAssemblyBuilder")
                    {
                        Stream licenseStream = a.GetManifestResourceStream(resource);

                        if (licenseStream != null)
                            return LoadLicenseFromStream(licenseStream);
                    }
                }

                return null;
            }
            catch
            {
                return null;
            }
		}

#if LICENSEASSEMBLY
		public static KrystalwareRuntimeLicense LoadLicenseFromStream(Stream s)
#else
		internal static KrystalwareRuntimeLicense LoadLicenseFromStream(Stream s)
#endif
		{
            try
            {
                XmlDocument doc = new XmlDocument();

                doc.Load(s);

                return new KrystalwareRuntimeLicense(doc);
            }
            catch
            {
                return null;
            }
		}

#if LICENSEASSEMBLY
		public static KrystalwareRuntimeLicense LoadLicenseFromString(string s)
#else
		internal static KrystalwareRuntimeLicense LoadLicenseFromString(string s)
#endif
		{
			XmlDocument doc = new XmlDocument();

			doc.LoadXml(s);

			return new KrystalwareRuntimeLicense(doc);
		}

#if LICENSEASSEMBLY
		public static KrystalwareRuntimeLicense LoadLicenseFromFile(Type t)
#else
		internal static KrystalwareRuntimeLicense LoadLicenseFromFile(Type t)
#endif
		{
            try
            {
                return LoadLicenseFromFile(t, Path.GetDirectoryName(t.Assembly.Location));
            }
            catch
            {
                return null;
            }
		}

#if LICENSEASSEMBLY
		public static KrystalwareRuntimeLicense LoadLicenseFromFile(string fileName)
#else
		internal static KrystalwareRuntimeLicense LoadLicenseFromFile(string fileName)
#endif
		{
			if (File.Exists(fileName))
			{
				FileStream s = null;

				try
				{
					s = File.OpenRead(fileName);

					return LoadLicenseFromStream(s);
				}
				finally
				{
					if (s != null)
						s.Close();
				}
			}
			else
			{
				return null;
			}
		}
#if LICENSEASSEMBLY
		public static KrystalwareRuntimeLicense LoadLicenseFromFile(Type t, string path)
#else
        internal static KrystalwareRuntimeLicense LoadLicenseFromFile(Type t, string path)
#endif
        {
            try
            {
                string assembly = t.Assembly.GetName().Name;
                string licenseFile = assembly + ".xml.lic";

                return LoadLicenseFromFile(Path.Combine(path, licenseFile));
            }
            catch
            {
                return null;
            }
        }
    }
}
