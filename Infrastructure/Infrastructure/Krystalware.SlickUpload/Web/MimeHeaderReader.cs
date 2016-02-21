using System;
using System.Collections.Specialized;
using System.Text;
using System.Web;

namespace Krystalware.SlickUpload.Web
{
	/// <summary>
	/// Summary description for MimeHeaderReader.
	/// </summary>
	internal sealed class MimeHeaderReader
	{
		enum HeaderReaderState
		{
			Reading,
			FoundFirstCR,
			FoundFirstLF,
			FoundSecondCR,
			FoundSecondLF
		}

		StringBuilder headers;
		Encoding encoding;

		public NameValueCollection Headers
        {
			get
			{
				NameValueCollection headersCollection = new NameValueCollection();

                string[] lines = headers.ToString().Split('\n');
				
				//HttpContext.Current.Response.Write(headers.ToString());

				for (int i = 0; i < lines.Length; i++)
				{
					string line = lines[i];

					int pos = line.IndexOf(':');
				
					// TODO: Handle continuations - lines starting with whitespace
					if (pos > 0)
						headersCollection[line.Substring(0, pos).Trim()] = line.Substring(pos + 1).Trim();
				}

				return headersCollection;
			}
		}

		bool _headerComplete;

		public bool HeaderComplete
		{
			get
			{
				return _headerComplete;
			}
		}

		public MimeHeaderReader(Encoding e)
		{
			encoding = e;

			Reset();
		}

		public void Reset()
		{
			headers = new StringBuilder();

			_headerComplete = false;
		}

		public int Read(byte[] buffer, int position)
		{
			int read = 0;

			HeaderReaderState state = HeaderReaderState.Reading;

			for (int i = position; i < buffer.Length && state != HeaderReaderState.FoundSecondLF; i++)
			{
				char current = (char)buffer[i];

				if (current == '\r')
				{
					switch (state)
					{
						case HeaderReaderState.Reading:
							state = HeaderReaderState.FoundFirstCR;

							break;
						case HeaderReaderState.FoundFirstCR:
						case HeaderReaderState.FoundFirstLF:
							state = HeaderReaderState.FoundSecondCR;

							break;
					}
				}
				else if (current == '\n')
				{
					switch (state)
					{
						case HeaderReaderState.Reading:
						case HeaderReaderState.FoundFirstCR:
							state = HeaderReaderState.FoundFirstLF;

							break;
						case HeaderReaderState.FoundFirstLF:
						case HeaderReaderState.FoundSecondCR:
							state = HeaderReaderState.FoundSecondLF;

							break;
					}
				}
				else
				{
					state = HeaderReaderState.Reading;
				}

				read++;
			}

			headers.Append(encoding.GetString(buffer, position, read));
			
			// If we found the blank line
			if (state == HeaderReaderState.FoundSecondLF)
			{
				_headerComplete = true;

				// Back off the last two lines
				string tail = headers.ToString(headers.Length - 4, 4);

				if (tail[2] == '\r')
					headers.Length -= 4;
				else if (tail[2] == '\n')
					headers.Length -= 2;
			}

			return read;
		}
	}
}