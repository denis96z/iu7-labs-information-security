using System;
using System.IO;
using System.Text;

namespace Lab3
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                if (args.Length != 2)
                {
                    throw new Exception("Параметры не указаны.");
                }

                string fileName = args[0];
                FileInfo fileInfo = new FileInfo(fileName);

                var data = File.ReadAllBytes(args[0]);
                var key = Encoding.UTF8.GetBytes(args[1]);

                var encrypted = new DES().EncryptData(data, key);
                var decrypted = new DES().DecryptData(encrypted, key);

                string encryptedFileName = fileInfo.DirectoryName + "\\" + fileInfo.Name.Substring(0,
                    fileInfo.Name.Length - fileInfo.Extension.Length) + "_encrypted" + fileInfo.Extension;
                string decryptedFileName = fileInfo.DirectoryName + "\\" + fileInfo.Name.Substring(0,
                    fileInfo.Name.Length - fileInfo.Extension.Length) + "_decrypted" + fileInfo.Extension;

                File.WriteAllBytes(encryptedFileName, encrypted);
                File.WriteAllBytes(decryptedFileName, decrypted);

                Console.WriteLine("DONE");
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            finally
            {
                Console.ReadLine();
            }
        }
    }
}
