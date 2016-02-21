using System;

namespace Infrasturcture.Cryptographer
{
    public class Md5Cryptographer
    {
        public static string GetGuidHash()
        {
            return Guid.NewGuid().ToString().GetHashCode().ToString("x");
        }

        public static string CreateHash(string plainText)
        {
            return Microsoft.Practices.EnterpriseLibrary.Security.Cryptography.Cryptographer.CreateHash("MD5Cng", plainText);
        }

        public static bool CompareHash(string plainText, string hashedText)
        {
            return Microsoft.Practices.EnterpriseLibrary.Security.Cryptography.Cryptographer.CompareHash("MD5Cng", plainText, hashedText);
        }

        

    }
}
