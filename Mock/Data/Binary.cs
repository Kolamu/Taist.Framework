namespace Mock.Data
{
    using System;
    using System.Text;
    using System.IO;
    public class Binary
    {
        public static void Print(string format, params object[] args)
        {
            string message = string.Format(format, args);
            using (StreamWriter sw = new StreamWriter("log.txt", true))
            {
                sw.WriteLine(message);
                sw.Flush();
            }
        }

        public static int ToInt(byte b1, byte b2)
        {
            int s1 = b2 << 8 | b1;
            return s1;
        }

        public static int ToInt(byte b1, byte b2, byte b3, byte b4)
        {
            int i1 = ToInt(b1, b2);
            int i2 = ToInt(b3, b4);
            return i2 << 16 | i1;
        }

        public static string ToString(byte[] data)
        {
            StringBuilder sb = new StringBuilder();
            foreach (byte b in data)
            {
                sb.Append((char)b);
            }
            return sb.ToString();
        }

        public static byte[] ToByte(long value)
        {
            byte[] bArray = new byte[4];
            string s = Convert.ToString(value, 2).PadLeft(32, '0');
            for (int i = 0; i < 4; i++)
            {
                bArray[i] = Convert.ToByte(s.Substring(8 * i, 8), 2);
            }
            Array.Reverse(bArray);
            return bArray;
        }

        public static byte[] ToByte(int value)
        {
            byte[] bArray = new byte[4];
            string s = Convert.ToString(value, 2).PadLeft(32, '0');
            for (int i = 0; i < 4; i++)
            {
                bArray[i] = Convert.ToByte(s.Substring(8 * i, 8), 2);
            }
            Array.Reverse(bArray);
            return bArray;
        }

        public static byte[] ToByte(short value)
        {
            byte[] bArray = new byte[2];
            string s = Convert.ToString(value, 2).PadLeft(16, '0');
            for (int i = 0; i < 2; i++)
            {
                bArray[i] = Convert.ToByte(s.Substring(8 * i, 8), 2);
            }
            Array.Reverse(bArray);
            return bArray;
        }

        public static byte[] ToByte(string value)
        {
            char[] cArray = value.ToCharArray();
            byte[] bArray = new byte[cArray.Length];
            for (int i = 0; i < cArray.Length; i++)
            {
                bArray[i] = (byte)cArray[i];
            }
            return bArray;
        }

        public static byte[] Combine(params object[] byteArray)
        {
            byte[] dst = new byte[0];
            foreach (object obj in byteArray)
            {
                byte[] nextByte = (byte[])obj;
                byte[] tmp = dst;
                dst = new byte[nextByte.Length + tmp.Length];
                Array.Copy(tmp, dst, tmp.Length);
                Array.Copy(nextByte, 0, dst, tmp.Length, nextByte.Length);
            }
            return dst;
        }
    }
}
