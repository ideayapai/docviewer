using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Documents.Utils
{
    public static class EncodingUtils
    {
        public static Encoding GetFileEncode(Stream stream)
        {
            BinaryReader br = new BinaryReader(stream);
            return GetFilEncoding(br.ReadBytes(2));
        }

        public static Encoding GetFilEncoding(byte[] buffer)
        {

            if (buffer[0] >= 0xEF)
            {
                if (buffer[0] == 0xEF && buffer[1] == 0xBB)
                {
                    return Encoding.UTF8;
                }
                else if (buffer[0] == 0xFE && buffer[1] == 0xFF)
                {
                    return Encoding.BigEndianUnicode;
                }
                else if (buffer[0] == 0xFF && buffer[1] == 0xFE)
                {
                    return Encoding.Unicode;
                }
                else
                {
                    return Encoding.Default;
                }
            }
            else
            {
                return Encoding.Default;
            }
        }

        public static Encoding GetFileEncode(string filename)
        {
            using (FileStream fs = new FileStream(filename, FileMode.Open, System.IO.FileAccess.Read))
            {
                BinaryReader br = new BinaryReader(fs);
                return GetFilEncoding(br.ReadBytes(2));
            }
        }
    }
}
