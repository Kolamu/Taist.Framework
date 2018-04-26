namespace Mock.Data
{
    using System;
    using System.Text;
    public class DecryptFactory
    {
        public static string Base64(string data, Encoding encoding = null)
        {
            if (encoding == null)
            {
                encoding = Encoding.Default;
            }
            byte[] dataBytes = Convert.FromBase64String(data);
            return encoding.GetString(dataBytes);
        }
    }
}
