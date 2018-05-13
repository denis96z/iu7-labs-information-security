using System;
using System.IO;
using System.Text;

namespace Lab4
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                if (args.Length != 5)
                {
                    throw new Exception("Invalid arguments.");
                }

                string fileName = args[0];
                string fileMode = args[4];

                FileInfo fileInfo = new FileInfo(fileName);
                string encryptedFileName = fileInfo.DirectoryName + "\\" + fileInfo.Name.Substring(0,
                    fileInfo.Name.Length - fileInfo.Extension.Length) + "_encrypted" + fileInfo.Extension;
                string decryptedFileName = fileInfo.DirectoryName + "\\" + fileInfo.Name.Substring(0,
                    fileInfo.Name.Length - fileInfo.Extension.Length) + "_decrypted" + fileInfo.Extension;

                (string, string) privateKey = (args[1], args[3]);
                (string, string) publicKey = (args[2], args[3]);

                if (fileMode == "/t")
                {
                    var data = File.ReadAllText(fileName, Encoding.UTF8);

                    var dataEncrypted = new RSA().EncryptText(data, publicKey);
                    var dataDecrypted = new RSA().DecryptText(dataEncrypted, privateKey);

                    File.WriteAllText(encryptedFileName, dataEncrypted, Encoding.UTF8);
                    File.WriteAllText(decryptedFileName, dataDecrypted, Encoding.UTF8);
                }
                else
                {
                    var data = File.ReadAllBytes(fileName);

                    var dataEncrypted = new RSA().EncryptBytes(data, publicKey);
                    var dataDecrypted = new RSA().DecryptBytes(dataEncrypted, privateKey);

                    File.WriteAllText(encryptedFileName, dataEncrypted, Encoding.UTF8);
                    File.WriteAllBytes(decryptedFileName, dataDecrypted);
                }
                
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }
}
