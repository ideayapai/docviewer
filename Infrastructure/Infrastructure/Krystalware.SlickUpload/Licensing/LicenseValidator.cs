using System;
using System.Security.Cryptography;
using System.Security.Cryptography.Xml;
using System.Xml;

namespace Krystalware.SlickUpload.Licensing
{
	/// <summary>
	/// Summary description for Class1.
	/// </summary>
#if LICENSEASSEMBLY
	public sealed class LicenseValidator
#else
	internal sealed class LicenseValidator
#endif
	{
		XmlDocument _doc;

		public bool IsValid(KeyInfo keyInfo)
		{
			SignedXml xml = new SignedXml(_doc);

			XmlNodeList nodeList = _doc.GetElementsByTagName("Signature");

			xml.LoadXml((XmlElement)nodeList[0]);

			xml.KeyInfo = keyInfo;

            xml.Resolver = null;
            
            return xml.CheckSignature();
		}

		public LicenseValidator(KrystalwareRuntimeLicense l)
		{
			if (l.LicenseKey == null)
				throw new ArgumentNullException("l");

			XmlDocument doc = new XmlDocument();

			doc.LoadXml(l.LicenseKey);

			_doc = doc;
		}

		public static KeyInfo GetKeyInfoFromXml(string xml)
		{
			// Use machine key store
			CspParameters cspParams = new CspParameters();
			cspParams.Flags = CspProviderFlags.UseMachineKeyStore;

			RSA rsa = new RSACryptoServiceProvider(cspParams);

			rsa.FromXmlString(xml);

			KeyInfo keyInfo = new KeyInfo();
			keyInfo.AddClause(new RSAKeyValue(rsa));

			return keyInfo;
		}
	}
}