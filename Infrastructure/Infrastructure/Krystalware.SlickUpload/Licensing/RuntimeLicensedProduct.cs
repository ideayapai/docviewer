using System;
using System.Xml;

namespace Krystalware.SlickUpload.Licensing
{
	/// <summary>
	/// Summary description for RuntimeLicensedProduct.
	/// </summary>
#if LICENSEASSEMBLY
	public sealed class RuntimeLicensedProduct
#else
	internal sealed class RuntimeLicensedProduct
#endif
	{
		readonly string _assemblyName;
		readonly string _version;
		readonly DateTime _expirationDate;
		readonly LicenseType _type;
		readonly LicenseScope _scope;
        readonly string _licenseUrl;

		public string AssemblyName
		{
			get
			{
				return _assemblyName;
			}
		}

		public string Version
		{
			get
			{
				return _version;
			}
		}

		public DateTime ExpirationDate
		{
			get
			{
				return _expirationDate;
			}
		}
		
		public LicenseType Type
		{
			get
			{
				return _type;
			}
		}

		public LicenseScope Scope
		{
			get
			{
				return _scope;
			}
		}

        public string LicenseUrl
        {
            get
            {
                return _licenseUrl;
            }
        }

		internal RuntimeLicensedProduct(XmlElement el)
		{
			_assemblyName = el.GetAttribute("AssemblyName");
			_version = el.GetAttribute("Version");
            _licenseUrl = el.GetAttribute("LicenseUrl");

			try
			{
				_expirationDate = DateTime.Parse(el.GetAttribute("ExpirationDate"));
			}
			catch
			{
				_expirationDate = DateTime.MaxValue;
			}
			
			try
			{
				_type = (LicenseType)Enum.Parse(typeof(LicenseType), el.GetAttribute("Type"));
			}
			catch
			{
				_type = LicenseType.Evaluation; 
			}

			try
			{
				_scope = (LicenseScope)Enum.Parse(typeof(LicenseScope), el.GetAttribute("Scope"));
			}
			catch
			{
				_scope = LicenseScope.WebSite; 
			}
		}

		internal RuntimeLicensedProduct(string assemblyName, string assemblyVersion)
		{
			_assemblyName = assemblyName;
			_version = assemblyVersion;
			_type = LicenseType.Evaluation;
			_scope = LicenseScope.WebSite;
			_expirationDate = DateTime.MaxValue;
		}
	}
}