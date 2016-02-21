using System;
using System.Collections;
using System.IO;
using System.Text;

namespace Krystalware.SlickUpload.Web
{
	/// <summary>
	/// Summary description for MimePushReader.
	/// </summary>
	internal abstract class PushReaderBase
	{
		protected Stream _stream;
		protected IMimePushHandler _handler;

		public PushReaderBase(Stream s, IMimePushHandler h)
		{
			_stream = s;
			_handler = h;
		}

		public abstract void Parse();
	}
}
