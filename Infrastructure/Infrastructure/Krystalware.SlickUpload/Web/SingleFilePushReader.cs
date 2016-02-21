using System;
using System.Collections;
using System.IO;
using System.Text;

namespace Krystalware.SlickUpload.Web
{
	/// <summary>
	/// Summary description for MimePushReader.
	/// </summary>
	internal sealed class SingleFilePushReader : PushReaderBase
	{
        public SingleFilePushReader(Stream s, IMimePushHandler h) :
            base(s, h)
		{
		}

		public override void Parse()
		{
            try
            {
                _handler.BeginPart(null);

                byte[] buffer = new byte[8192];
                int read;

                while ((read = _stream.Read(buffer, 0, buffer.Length)) > 0)
                    _handler.PartData(ref buffer, 0, read);

                _handler.EndPart(true, true);
            }
            catch
            {
                _handler.EndPart(false, false);

                throw;
            }
		}
	}
}
