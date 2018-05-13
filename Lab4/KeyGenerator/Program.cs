using System;

namespace KeyGenerator
{
    class Program
    {
        static void Main(string[] args)
        {
            RSA rsa = new RSA();

            string genNext = String.Empty;
            do
            {
                rsa.GenerateKeys();
                Console.WriteLine("Public key: ({0}, {1})", rsa.PublicKey.Item1, rsa.PublicKey.Item2);
                Console.WriteLine("Private key: ({0}, {1})", rsa.PrivateKey.Item1, rsa.PrivateKey.Item2);

                Console.Write("\r\nNew key (y/n): ");
                genNext = Console.ReadLine();
                Console.WriteLine();
            }
            while (genNext == "y");
        }
    }
}
