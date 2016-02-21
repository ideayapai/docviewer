using System.ComponentModel;
using System.Xml;

namespace Krystalware.SlickUpload.Licensing
{
	/// <summary>
	/// Summary description for RuntimeLicense.
	/// </summary>
#if LICENSEASSEMBLY
	public sealed class KrystalwareRuntimeLicense : License
#else
	internal sealed class KrystalwareRuntimeLicense : License
#endif
	{
		readonly string _licenseKey;
		readonly string _customerName;

		readonly RuntimeLicensedProduct[] _licensedProducts;

		public string CustomerName
		{
			get
			{
				return _customerName;
			}
		}

		public RuntimeLicensedProduct[] LicensedProducts
		{
			get
			{
				return _licensedProducts;
			}
		}

		internal KrystalwareRuntimeLicense(XmlDocument doc)
		{
			_licenseKey = doc.OuterXml;

			foreach (XmlElement el in doc.DocumentElement.ChildNodes)
			{
				switch (el.Name)
				{
					case "Customer":
						_customerName = el.GetAttribute("Name");

						break;
					case "Products":
						RuntimeLicensedProduct[] products = new RuntimeLicensedProduct[el.ChildNodes.Count];

						int i = 0;

						foreach (XmlElement prodEl in el.ChildNodes)
						{
							products[i] = new RuntimeLicensedProduct(prodEl);

							i++;
						}

						_licensedProducts = products;

						break;
				}
			}
		}

		internal KrystalwareRuntimeLicense(string assemblyName, string assemblyVersion)
		{
			_licenseKey = null;
			_customerName = "Evaluation License";

			_licensedProducts = new RuntimeLicensedProduct[] {new RuntimeLicensedProduct(assemblyName, assemblyVersion)};
		}

		public override string LicenseKey
		{
			get
			{
				return _licenseKey;
			}
		}

		public override void Dispose()
		{

		}
	}
}