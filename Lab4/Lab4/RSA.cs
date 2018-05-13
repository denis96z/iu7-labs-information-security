using System;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Lab4
{
    class RSA
    {
        public string EncryptText(string text, (string, string) key)
        {
            BigInteger exp = BigInteger.Parse(key.Item1);
            BigInteger n = BigInteger.Parse(key.Item2);

            StringBuilder newText = new StringBuilder();
            for (int i = 0; i < text.Length; i++)
            {
                BigInteger c = new BigInteger(text[i]);
                BigInteger code = BigInteger.ModPow(c, exp, n);
                newText.Append(code.ToString() + " ");
            }

            return newText.ToString();
        }

        public string DecryptText(string text, (string, string) key)
        {
            BigInteger dxp = BigInteger.Parse(key.Item1);
            BigInteger n = BigInteger.Parse(key.Item2);

            var codes = text.Split(' ', '\r', '\n', '\0');

            StringBuilder newText = new StringBuilder();
            for (int i = 0; i < codes.Length - 1; i++)
            {
                BigInteger code = BigInteger.Parse(codes[i]);
                BigInteger c = BigInteger.ModPow(code, dxp, n);
                newText.Append((char)int.Parse(c.ToString()));
            }

            return newText.ToString();
        }

        public string EncryptBytes(byte[] bytes, (string, string) key)
        {
            BigInteger exp = BigInteger.Parse(key.Item1);
            BigInteger n = BigInteger.Parse(key.Item2);

            StringBuilder newText = new StringBuilder();
            for (int i = 0; i < bytes.Length; i++)
            {
                BigInteger c = new BigInteger(bytes[i]);
                BigInteger code = BigInteger.ModPow(c, exp, n);
                newText.Append(code.ToString() + " ");
            }

            return newText.ToString();
        }

        public byte[] DecryptBytes(string text, (string, string) key)
        {
            BigInteger dxp = BigInteger.Parse(key.Item1);
            BigInteger n = BigInteger.Parse(key.Item2);

            var codes = text.Split(' ', '\r', '\n', '\0');

            byte[] bytes = new byte[codes.Length - 1];
            for (int i = 0; i < codes.Length - 1; i++)
            {
                BigInteger code = BigInteger.Parse(codes[i]);
                BigInteger c = BigInteger.ModPow(code, dxp, n);
                bytes[i] = byte.Parse(c.ToString());
            }

            return bytes;
        }
    }
}
