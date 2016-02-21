using System;

namespace Krystalware.SlickUpload.Blobify.S3
{
    public class S3Exception : Exception
    {
        public string Code { get; private set; }
        public string ResponseXml { get; private set; }

        public S3Exception(string message, string code, string responseXml, Exception innerException)
            : base(message, innerException)
        {
            Code = code;
            ResponseXml = responseXml;
        }
    }
}
