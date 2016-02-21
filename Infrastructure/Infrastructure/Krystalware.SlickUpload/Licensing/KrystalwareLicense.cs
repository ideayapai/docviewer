using System;
using System.ComponentModel;
using System.IO;
using System.Security.Cryptography;
using System.Security.Cryptography.Xml;
using System.Xml;
using AlpineSoft;

namespace Krystalware.Licensing
{
	/// <summary>
	/// Summary description for License.
	/// </summary>
	public sealed class KrystalwareLicense
	{
		string _customerName;

		LicensedProduct[] _licensedProducts;

		public string CustomerName
		{
			get
			{
				return _customerName;
			}
			set
			{
				_customerName = value;
			}
		}

        public int OrderId { get; set; }

		public LicensedProduct[] LicensedProducts
		{
			get
			{
				return _licensedProducts;
			}
			set
			{
				_licensedProducts = value;
			}
		}

		public KrystalwareLicense()
		{
			//
			// TODO: Add constructor logic here
			//
		}

		public KrystalwareLicense(XmlDocument doc)
		{
			foreach (XmlElement el in doc.DocumentElement.ChildNodes)
			{
				switch (el.Name)
				{
					case "Customer":
						_customerName = el.GetAttribute("Name");

						break;
                    case "OrderId":
                        OrderId = Convert.ToInt32(el.GetAttribute("OrderId"));

                        break;
                    case "Products":
						LicensedProduct[] products = new LicensedProduct[el.ChildNodes.Count];

						int i = 0;

						foreach (XmlElement prodEl in el.ChildNodes)
						{
							products[i] = new LicensedProduct(prodEl);

							i++;
						}

						LicensedProducts = products;

						break;
				}
			}
		}

		public XmlDocument Save()
		{
			XmlDocument doc = new XmlDocument();

			XmlElement rootEl = doc.CreateElement("License");

			rootEl.SetAttribute("Version", "3.0");

			doc.AppendChild(rootEl);

			XmlElement customerEl = doc.CreateElement("Customer");

			customerEl.SetAttribute("Name", CustomerName);
            customerEl.SetAttribute("OrderId", OrderId.ToString());

			rootEl.AppendChild(customerEl);

			XmlElement productsEl = doc.CreateElement("Products");

			foreach (LicensedProduct product in _licensedProducts)
				product.Save(productsEl);

			rootEl.AppendChild(productsEl);

			return doc;
		}

        //public static bool VeryBad(string sTmp)
        //{
        //    int[] arrayChars = new int[] { 38, 60, 62, 34, 61, 39 };

        //    for (int iCount = 0; iCount < arrayChars.Length; iCount++)
        //    {
        //        if (sTmp.Contains(arrayChars[iCount]).ToString()) return true;
        //        //sTmp = sTmp.Replace(((char)arrayChars[iCount]).ToString(), "&#" + arrayChars[iCount].ToString() + ";");
        //    }

        //    return false;
        //}

        public static string Tag(string sTmp)
        {
            string strReturn = sTmp;
            int[] arrayChars = new int[] { 38, 60, 62, 34, 61, 39 };

            for (int iCount = 0; iCount < arrayChars.Length; iCount++)
            {
                string strBad = ((char)arrayChars[iCount]).ToString();

                if (strReturn.Contains(strBad))
                { 
                    strReturn = strReturn.Replace(strBad, "~~~" + strBad + "~~~");
                }
            }
            return strReturn;
        }

        public static string XmlEncode(string sTmp)
        {
            int[] arrayChars = new int[] { 38, 60, 62, 34, 61, 39 };

            for (int iCount = 0; iCount < arrayChars.Length; iCount++)
            {
                sTmp = sTmp.Replace(((char)arrayChars[iCount]).ToString(), "&#" + arrayChars[iCount].ToString() + ";");
            }
            return sTmp;
        }

        public XmlDocument SaveWithSignature(string keyFileName)
        {
//            CspParameters cspParams = new CspParameters();
//            cspParams.Flags = CspProviderFlags.UseExistingKey;

//            RSA rsa = new RSACryptoServiceProvider(cspParams);
            EZRSA rsa = new EZRSA(1024);

            rsa.FromXmlString(File.ReadAllText(keyFileName));

            return SaveWithSignature(rsa);
        }
        
        public void SaveWithSignature(Stream stream, string keyFileName)
		{
			RSA rsa = new RSACryptoServiceProvider();

			if (File.Exists(keyFileName))
			{
				StreamReader sr = File.OpenText(keyFileName);
				string str = sr.ReadToEnd();

				rsa.FromXmlString(str);

				sr.Close();

				XmlDocument doc = SaveWithSignature(rsa);

				doc.Save(stream);
			}

            // TODO: exception
			/*else
			{
				int poo;
			}*/
		}
		public void SaveWithSignature(string fileName, string keyFileName)
		{
			EZRSA rsa = new EZRSA(1024);

            //rsa.MapHashAlgorithmOID(

            if (File.Exists(keyFileName))
            {
                StreamReader sr = File.OpenText(keyFileName);
                string str = sr.ReadToEnd();
                
                rsa.FromXmlString(str);

                sr.Close();
                
                XmlDocument doc = SaveWithSignature(rsa);

                doc.Save(fileName);
            }

            // TODO: exception
            /*else
            {
                int poo;
            }*/
			
		}

		public XmlDocument SaveWithSignature(AsymmetricAlgorithm key)
		{
			XmlDocument doc = Save();
	
			SignedXml sign = new SignedXml(doc);

			Reference reference = new Reference();
			reference.Uri = "";

			Transform trns = new XmlDsigC14NTransform();
			reference.AddTransform(trns);

			XmlDsigEnvelopedSignatureTransform env = new XmlDsigEnvelopedSignatureTransform();
			reference.AddTransform(env);

			sign.AddReference(reference);

			sign.SigningKey = key;
            sign.Resolver = null;

			sign.ComputeSignature();

			XmlElement xmlDigitalSignature = sign.GetXml();

			doc.DocumentElement.AppendChild(doc.ImportNode(xmlDigitalSignature, true));
                
			return doc;
		}
	}
}