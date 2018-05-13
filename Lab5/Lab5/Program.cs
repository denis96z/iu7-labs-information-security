using System;
using System.IO;
using System.Windows.Forms;

namespace Lab5
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                if (args.Length == 1 && args[0] == "help")
                {
                    string exeName = Path
                        .GetFileNameWithoutExtension(Application.ExecutablePath);

                    Console.WriteLine("Создание подписи:");
                    Console.WriteLine(exeName + " /s имя_файла -o имя_файла_подписи" + "\n");
                    Console.WriteLine("Проверка подписи:");
                    Console.WriteLine(exeName + " /v имя_файла -s имя_файла_подписи" + "\n");
                }
                else if (args.Length == 4)
                {
                    if (args[0].ToLower() == "/s")
                    {
                        string sourceFileName = args[1];
                        FileInfo fileInfo = new FileInfo(sourceFileName);

                        if (args[2] != "-o")
                        {
                            ThrowArgumentException(args[2]);
                        }

                        string signatureFileName = args[3];
                        fileInfo = new FileInfo(signatureFileName);

                        DigitalSignature digitalSignature = new DigitalSignature();
                        digitalSignature.CreateSignatureFile(sourceFileName, signatureFileName);

                        Console.WriteLine("Файл подписи создан.");
                    }
                    else if (args[0].ToLower() == "/v")
                    {
                        string sourceFileName = args[1];
                        FileInfo fileInfo = new FileInfo(sourceFileName);

                        if (args[2] != "-s")
                        {
                            ThrowArgumentException(args[2]);
                        }

                        string signatureFileName = args[3];
                        fileInfo = new FileInfo(signatureFileName);

                        DigitalSignature digitalSignature = new DigitalSignature();
                        bool f = digitalSignature.VerifySignature(sourceFileName, signatureFileName);

                        Console.WriteLine(f ? "Подпись действительна." : "Подпись недействительна.");
                    }
                    else
                    {
                        ThrowArgumentException(args[0]);
                    }
                }
                else
                {
                    ThrowArgumentsNumberException();
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.Message);
            }
        }

        static void ThrowArgumentException(string arg)
        {
            throw new ArgumentException("Недопустимый параметр \"" + arg + "\".");
        }

        static void ThrowArgumentsNumberException()
        {
            throw new ArgumentException("Неверное число аргументов.");
        }
    }
}
