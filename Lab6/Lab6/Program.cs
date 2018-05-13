using System;
using System.IO;

namespace Lab6
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                if (args.Length != 1)
                {
                    throw new Exception("Параметры не указаны.");
                }

                string fileName = args[0];
                FileInfo fileInfo = new FileInfo(fileName);

                var data = File.ReadAllBytes(args[0]);

                var compressed = new Huffman().Compress(data);
                var decompressed = new Huffman().Decompress(compressed);

                string compressedFileName = fileInfo.DirectoryName + "\\" + fileInfo.Name.Substring(0,
                    fileInfo.Name.Length - fileInfo.Extension.Length) + "_compressed" + fileInfo.Extension;
                string decompressedFileName = fileInfo.DirectoryName + "\\" + fileInfo.Name.Substring(0,
                    fileInfo.Name.Length - fileInfo.Extension.Length) + "_decompressed" + fileInfo.Extension;
                File.WriteAllBytes(compressedFileName, compressed);
                File.WriteAllBytes(decompressedFileName, decompressed);

                Console.WriteLine("Выполнено");
                Console.WriteLine("Размер исходного файла: " + data.Length);
                Console.WriteLine("Размер сжатого файла:   " + compressed.Length);

                /*double k = (double)data.Length / compressed.Length;
                Console.WriteLine("Коэффициент сжатия: {0:0.0000}", k);*/
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }
}
