using System;
using System.Linq;
using KeyGenerator.Properties;
using System.Text;

namespace KeyGenerator
{
    class RSA
    {
        private long exp = 0, dxp = 0, n = 0;

        private Random random = new Random();

        private readonly long[] PRIME_NUMBERS;
        private readonly long[] F_PRIME_NUMBERS =
        {
            17, 257, 65537
        };

        public RSA()
        {
            var pNumbers = from num in Resources.PrimeNumbers.Split('\t', '\r', '\n', ' ', '\0')
                           where num != String.Empty
                           select num;

            PRIME_NUMBERS = new long[Enumerable.Count(pNumbers)];

            int index = 0;
            foreach (string num in pNumbers)
            {
                PRIME_NUMBERS[index++] = long.Parse(num);
            }

            GenerateKeys();
        }

        public void GenerateKeys()
        {
            long p = PRIME_NUMBERS[random.Next(PRIME_NUMBERS.Length)];
            long q = p;
            do
            {
                q = PRIME_NUMBERS[random.Next(PRIME_NUMBERS.Length)];
            }
            while (q == p);

            n = p * q;
            long f = LCM((p - 1), (q - 1));

            exp = F_PRIME_NUMBERS[random.Next(F_PRIME_NUMBERS.Length)];
            dxp = ModInverse(exp, f);
        }

        public (long, long) PublicKey
        {
            get { return (exp, n); }
        }

        public (long, long) PrivateKey
        {
            get { return (dxp, n); }
        }

        long GCD(long a, long b)
        {
            while (true)
            {
                if (a == 0) return b;
                b %= a;
                if (b == 0) return a;
                a %= b;
            }
        }

        long LCM(long a, long b)
        {
            long temp = GCD(a, b);
            return temp != 0 ? (a / temp * b) : 0;
        }

        public long ModInverse(long e, long fn)
        {
            long d = 1;
            while (true)
            {
                if ((e * d) % fn == 1)
                    break;
                else
                    d++;
            }
            return d;
        }
    }
}
