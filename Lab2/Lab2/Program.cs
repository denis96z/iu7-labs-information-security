using System;
using System.IO;
using System.Text;

namespace Lab2
{
    class Program
    {
        private static readonly Encoding ENCODING = Encoding.Unicode;

        static void Main(string[] args)
        {
            try
            {
                Enigma enigma = new Enigma(ENCODING);

                string fileName = args.Length == 1 ?
                    args[0] : throw new Exception("Файл не указан.");

                FileInfo fileInfo = new FileInfo(fileName);

                string encryptedFileName = fileInfo.DirectoryName + "\\" + fileInfo.Name.Substring(0,
                    fileInfo.Name.Length - fileInfo.Extension.Length) + "_encrypted" + fileInfo.Extension;

                string decryptedFileName = fileInfo.DirectoryName + "\\" + fileInfo.Name.Substring(0,
                    fileInfo.Name.Length - fileInfo.Extension.Length) + "_decrypted" + fileInfo.Extension;

                //var file = File.ReadAllBytes(fileName);
                string text = File.ReadAllText(fileName, ENCODING);

                enigma.Initialize();
                string encryptedText = enigma.EncryptText(text);

                Console.WriteLine("Encrypted...");

                enigma.Initialize();
                string decryptedText = enigma.EncryptText(encryptedText);

                Console.WriteLine("Decrypted...");

                if (text != decryptedText)
                {
                    Console.WriteLine("Encryption error...");
                }

                File.WriteAllText(encryptedFileName, encryptedText, ENCODING);
                File.WriteAllText(decryptedFileName, decryptedText, ENCODING);

                Console.WriteLine("Done. Press any key to finish...");
                Console.ReadLine();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Console.ReadLine();
            }
        }
    }
}
