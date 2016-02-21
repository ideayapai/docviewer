using System;

namespace Krystalware.Licensing
{
	/// <summary>
	/// Summary description for LicensePublicKeyAttribute.
	/// </summary>
	[AttributeUsage(AttributeTargets.Assembly)]
#if LICENSEASSEMBLY
	public class LicensePublicKeyAttribute : Attribute
#else
	internal class LicensePublicKeyAttribute : Attribute
#endif
		{
		string _xmlString;

		public string XmlString
		{
			get
			{
				return _xmlString;
			}
		}

		public LicensePublicKeyAttribute(string xmlString)
		{
			_xmlString = xmlString;
		}
	}
}