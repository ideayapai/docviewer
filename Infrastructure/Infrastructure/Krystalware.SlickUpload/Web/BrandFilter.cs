using System;
using System.IO;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Web.UI;

namespace Krystalware.SlickUpload.Web
{
	internal sealed class BrandFilter : Stream
	{
		Stream _baseStream;
		Encoding _encoding;
		StringBuilder _response = new StringBuilder();
        string _scheme;

		public BrandFilter(Stream baseStream, Encoding encoding, string scheme)
		{
			_baseStream = baseStream;
			_encoding = encoding;
            _scheme = scheme;
		}

		public override bool CanRead
		{
			get
			{
				return false;
			}
		}

		public override bool CanSeek
		{
			get
			{
				return false;
			}
		}

		public override bool CanWrite
		{
			get
			{
				return false;
			}
		}

		public override void Close()
		{
			//Regex xInput = new Regex(@"type\s*=[\s|""]*file", RegexOptions.IgnoreCase | RegexOptions.Singleline);

			//if (xInput.IsMatch(_response.ToString()))
			//{
				// Insert the brand
				Regex xBody = new Regex("</head>", RegexOptions.IgnoreCase);

				int location;
			
				Match m = xBody.Match(_response.ToString());

				if (m.Success)
					location = m.Index;
				else
					location = _response.Length;

				_response.Insert(location, GetBrand());
			//}

			byte[] data = _encoding.GetBytes(_response.ToString());

			_baseStream.Write(data, 0, data.Length);
			_baseStream.Close();
		}

		string GetBrand()
		{
            return "<script type=\"text/javascript\">window._kwInit = window._kwInit || []; window._kwInit.push({ isLicensed: false" +
                                   ", brandLocation: \"" + (SlickUploadContext.Config.BrandLocation == BrandLocation.BottomRight ? "bottom-right" : "inline") +
                                   "\", version: \"" + Assembly.GetExecutingAssembly().GetName().Version.ToString() +
                                   "\", brandUrl: \"" + new Page().ClientScript.GetWebResourceUrl(typeof(BrandFilter), "Krystalware.SlickUpload.Resources.PoweredBy.png") + "\" });</script>";

            /*return "<div style=\"z-index:99999;background-color:#265ecf;border-top:1px solid #37b0e5;border-right:1px solid #37b0e5;position:absolute;position:fixed;right:0;bottom:0;margin:0;padding:.25em .5em .25em .5em;font-size:80%;font-family:Calibri,Verdana,Arial,sans-serif;background-image:url('" + _scheme + "://krystalware.com/brandping?version=" + GetVersion() + "')\">" +
				        "<a href=\"http://krystalware.com/slickupload\" style=\"color:#ffffff;text-decoration:none;\" target=\"_top\" onmouseover=\"this.style.textDecoration='underline'\" onmouseout=\"this.style.textDecoration='none'\">" +
							"Powered By SlickUpload Evaluation Version" + 
						"</a>" +
					 "</div>";*/
		}

        string GetVersion()
        {
            return Assembly.GetExecutingAssembly().GetName().Version.ToString();
        }

		public override void Flush()
		{
			//_responseStream.Flush();
		}

		public override long Length
		{
			get
			{
				throw new NotSupportedException();
			}
		}

		public override long Position
		{
			get
			{
				throw new NotSupportedException();
			}
			set
			{
				throw new NotSupportedException();
			}
		}

		public override long Seek(long offset, SeekOrigin origin)
		{
			throw new NotSupportedException();
		}

		public override void SetLength(long length)
		{
			throw new NotSupportedException();
		}

		public override int Read(byte[] buffer, int offset, int count)
		{
			throw new NotSupportedException();
		}
		
		public override void Write(byte[] buffer, int offset, int count)
		{
			_response.Append(_encoding.GetString(buffer, offset, count));	
		}
	}
}
