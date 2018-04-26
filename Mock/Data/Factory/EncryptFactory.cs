namespace Mock.Data
{
    using System;
    using System.Text;
    using System.Security.Cryptography;
    public class EncryptFactory
    {
        public static string Base64(string data, Encoding encoding = null)
        {
            if(encoding == null)
            {
                encoding = Encoding.Default;
            }
            byte[] dataBytes = encoding.GetBytes(data);
            return Convert.ToBase64String(dataBytes);
        }

        public static byte[] Md5(string data, Encoding encoding = null)
        {
            byte[] result = DataFactory.GetEncoding(encoding).GetBytes(data);
            MD5 md5 = new MD5CryptoServiceProvider();
            return md5.ComputeHash(result);
        }
    }
}
