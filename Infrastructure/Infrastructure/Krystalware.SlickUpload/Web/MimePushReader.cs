using System;
using System.Collections;
using System.IO;
using System.Text;

namespace Krystalware.SlickUpload.Web
{
	/// <summary>
	/// Summary description for MimePushReader.
	/// </summary>
	internal sealed class MimePushReader : PushReaderBase
	{
		byte[] _boundary;
		Encoding _encoding;

        byte[] _buffer = new byte[1024 * 8];
        byte _firstBoundaryByte;

        enum MimeReaderState
		{
			ReadingHeaders,
			ReadingBody,
			Finished
		}

		public MimePushReader(Stream s, IMimePushHandler h, byte[] b, Encoding e) :
            base(s, h)
		{
			_boundary = b;
			_encoding = e;

            _firstBoundaryByte = _boundary[0];
		}

		public override void Parse()
		{
            try
            {
                MimeReaderState state = MimeReaderState.ReadingHeaders;
                MimeHeaderReader headerReader = new MimeHeaderReader(_encoding);

                // Read the first chunk
                int read = _stream.Read(_buffer, 0, _buffer.Length);

                // Skip past the first boundary
                int position = IndexOf(0, read) + _boundary.Length;

                // If the end is here, we're done (empty request)
                if (_encoding.GetString(_buffer, position, 2) == "--" && (char)_buffer[position + 3] == '\r' || (char)_buffer[position + 3] == '\n')
                    return;

                // Skip past the end line
                if ((char)_buffer[position] == '\r')
                    position += 2;
                else if ((char)_buffer[position] == '\n')
                    position += 1;

                int zeroReads = 0;

                while (state != MimeReaderState.Finished)
                {
                    switch (state)
                    {
                        case MimeReaderState.ReadingHeaders:
                            position += headerReader.Read(_buffer, position);

                            if (headerReader.HeaderComplete)
                            {
                                state = MimeReaderState.ReadingBody;

                                _handler.BeginPart(headerReader.Headers);

                                headerReader.Reset();
                            }

                            break;
                        case MimeReaderState.ReadingBody:
                            int boundaryPos = IndexOf(position, read);

                            // If block ends with \r or \n or a combo, consider that a boundary start
                            if (boundaryPos == -1)
                            {
                                if (read > 1 && _buffer[read - 2] == '\r')
                                    boundaryPos = read - 2;
                                else if (read > 0 && (_buffer[read - 1] == '\r' || _buffer[read - 1] == '\n'))
                                    boundaryPos = read - 1;
                            }

                            // If boundary not found
                            if (boundaryPos == -1)
                            {
                                _handler.PartData(ref _buffer, position, read - position);

                                position += read - position;
                            }
                            else
                            {
                                int actualLength;

                                actualLength = boundaryPos - position;

                                if (actualLength >= 2)
                                {
                                    if ((char)_buffer[boundaryPos - 2] == '\r')
                                        actualLength -= 2;
                                    else if ((char)_buffer[boundaryPos - 2] == '\n')
                                        actualLength -= 1;
                                }

                                _handler.PartData(ref _buffer, position, actualLength);

                                // Check for end
                                if (boundaryPos <= read - (_boundary.Length + 2))
                                {
                                    // Check for "--" which means last part
                                    bool isLast = (_encoding.GetString(_buffer, boundaryPos + _boundary.Length, 2) == "--");

                                    if (isLast)
                                    {
                                        state = MimeReaderState.Finished;
                                    }
                                    else
                                    {
                                        state = MimeReaderState.ReadingHeaders;
                                    }

                                    _handler.EndPart(isLast, true);

                                    position += (boundaryPos + _boundary.Length - position + 2);
                                }
                                // Boundary laps over
                                else
                                {
                                    // Also take the possible cr/lf combo removed above
                                    boundaryPos -= ((boundaryPos - position) - actualLength);

                                    int boundaryFragmentLength = read - boundaryPos;

                                    // Move the boundary data to the front of the buffer
                                    Buffer.BlockCopy(_buffer, boundaryPos, _buffer, 0, boundaryFragmentLength);

                                    // Load in more data
                                    read = _stream.Read(_buffer, boundaryFragmentLength, _buffer.Length - boundaryFragmentLength);

                                    // Reset position
                                    position = 0;

                                    // Adjust read count because of existing data in buffer
                                    read += boundaryFragmentLength;
                                }
                            }

                            break;
                    }

                    // Get more data if we need it
                    if (state != MimeReaderState.Finished)
                    {
                        if (position >= read)
                        {
                            read = _stream.Read(_buffer, 0, _buffer.Length);

                            position = 0;

                            if (read == 0)
                            {
                                if (zeroReads == 10)
                                    throw new UploadDisconnectedException();

                                zeroReads++;

                                System.Threading.Thread.Sleep(100);
                            }
                            else
                            {
                                zeroReads = 0;
                            }
                        }
                    }
                }
            }
            catch (UploadCancelledException)
            {
                throw;
            }
            catch
            {
                _handler.EndPart(false, false);

                throw;
            }
        }
       
        /*int IndexOf(int start, int end)
        {
            if (start == 0 && end == buffer.Length)
            {
                int matchedCount = 0;

                for (int i = 0; i < buffer.Length; i++)
                {
                    if (matchedCount == 0)
                    {
                        if (buffer[i] == prefix)
                            matchedCount++;
                    }
                    else
                    {
                        if (buffer[i] == boundary[matchedCount])
                        {
                            matchedCount++;

                            if (matchedCount == boundary.Length)
                                return i - matchedCount;
                        }
                        else
                            matchedCount = 0;
                    }
                }

                return -1;
            }
            else
            {
                int matchedCount = 0;

                for (int i = start; i < buffer.Length; i++)
                {
                    if (i == end)
                        return (matchedCount > 0 ? i - 1 - matchedCount : -1);

                    if (matchedCount == 0)
                    {
                        if (buffer[i] == prefix)
                            matchedCount++;
                    }
                    else
                    {
                        if (buffer[i] == boundary[matchedCount])
                        {
                            matchedCount++;

                            if (matchedCount == boundary.Length)
                                return i - matchedCount;
                        }
                        else
                            matchedCount = 0;
                    }
                }

                return -1;
            }
        }*/

        int IndexOf(int start, int end)
		{
            unchecked
            {
                int matched = 0;
                int matchStart = Array.IndexOf<byte>(_buffer, _firstBoundaryByte, start, end - start);

                if (matchStart != -1)
                {
                    while (matchStart + matched < end)
                    {
                        // If the buffer is still matching the pattern
                        if (_buffer[matchStart + matched] == _boundary[matched])
                        {
                            // Continue matching
                            matched++;

                            // If we matched the whole thing, we're done
                            if (matched == _boundary.Length)
                                break;
                        }
                        else
                        {
                            matchStart += matched;

                            matchStart = Array.IndexOf<byte>(_buffer, _firstBoundaryByte, matchStart, end - matchStart);

                            if (matchStart == -1)
                                break;
                            else
                                matched = 0;
                        }
                    }
                }

                return matchStart;
            }
		}		

/*        int[] BuildSearchTable(byte[] pattern)
                {
                    int[] t = new int[pattern.Length];
                    int tableI = 2;
                    int j = 0;
                    t[0] = -1;
                    t[1] = 0;
                    while (tableI < pattern.Length)
                    {
                        if (pattern[tableI - 1] == pattern[j])
                        {
                            t[tableI] = j + 1;
                            j++;
                            tableI++;
                        }
                        else if (j > 0)
                        {
                            j = t[j];
                        }
                        else
                        {
                            t[tableI] = 0;
                            tableI++;
                            j = 0;
                        }
                    }

                    return t;
                }

                int IndexOf(int start, int end)
                {

                    int m = start;
                    int i = 0;
                    while (((m + i) < end) && (i < boundary.Length))
                    {
                        if (buffer[m + i] == boundary[i]) i++;
                        else
                        {
                            m += i - _searchTable[i];
                            if (i > 0) i = _searchTable[i];
                            i++;
                        }
                    }
                    if (i == boundary.Length) return m;
                    else return -1;
                }
*/        
        /*public int IndexOf(byte[] buffer, byte[] pattern, int start, int end)
        {
            int m = start;
            int i = start;
            int[] t = new int[pattern.Length];

            while (((m + i) < end) && (i < pattern.Length))
            {
                if (buffer[m + i] == pattern[i]) i++;
                else
                {
                    m += i - t[i];
                    if (i > 0) i = t[i];
                    i++;
                }
            }
            if (i == pattern.Length) return m;
            else return -1;
        }*/
	}
}
