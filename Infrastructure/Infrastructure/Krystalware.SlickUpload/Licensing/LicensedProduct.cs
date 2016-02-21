using System;
using System.Xml;

namespace Krystalware.Licensing
{
	/// <summary>
	/// Summary description for LicensedProduct.
	/// </summary>
	public class LicensedProduct
	{		
		string _assemblyName;
		string _version;
		DateTime _expirationDate;
        
		LicenseType _type;
		LicenseScope _scope;

        public string LicenseUrl { get; set; }

		public string AssemblyName
		{
			get
			{
				return _assemblyName;
			}
			set
			{
				_assemblyName = value;
			}
		}

		public string Version
		{
			get
			{
				return _version;
			}
			set
			{
				_version = value;
			}
		}

		public DateTime ExpirationDate
		{
			get
			{
				return _expirationDate;
			}
			set
			{
				_expirationDate = value;
			}
		}
		
		public LicenseType Type
		{
			get
			{
				return _type;
			}
			set
			{
				_type = value;
			}
		}

		public LicenseScope Scope
		{
			get
			{
				return _scope;
			}
			set
			{
				_scope = value;
			}
		}

		public LicensedProduct()
		{
			//
			// TODO: Add constructor logic here
			//
		}

		public LicensedProduct(XmlElement el)
		{
			_assemblyName = el.GetAttribute("AssemblyName");
			_version = el.GetAttribute("Version");
            LicenseUrl = el.GetAttribute("LicenseUrl");

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
		
		public void Save(XmlElement parent)
		{
			XmlElement el = parent.OwnerDocument.CreateElement("Product");

			el.SetAttribute("AssemblyName", _assemblyName);
			el.SetAttribute("Version", _version);
			el.SetAttribute("ExpirationDate", _expirationDate.ToString("yyyy-MM-dd"));
			el.SetAttribute("Type", _type.ToString());
			el.SetAttribute("Scope", _scope.ToString());

            if (!string.IsNullOrEmpty(LicenseUrl))
                el.SetAttribute("LicenseUrl", LicenseUrl);

			parent.AppendChild(el);
		}
	}
}
