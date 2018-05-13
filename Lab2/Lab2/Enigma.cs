using System;
using System.Text;

namespace Lab2
{
    class Rotor
    {
        private int numRotations = 0;
        private readonly int rotationsPeriod = 1;

        private char[] symbols = null;
        private readonly char[] initialSymbols = null;

        public Rotor(int p, int numSymbols)
        {
            rotationsPeriod = p;

            symbols = new char[numSymbols];
            initialSymbols = new char[numSymbols];

            for (int i = 0; i < initialSymbols.Length; i++)
            {
                initialSymbols[i] = (char)i;
            }

            Random r = new Random();
            for (int i = initialSymbols.Length - 1; i > 0; i--)
            {
                int j = r.Next(i);
                char t = initialSymbols[i];
                initialSymbols[i] = initialSymbols[j];
                initialSymbols[j] = t;
            }
        }

        public void Initialize()
        {
            numRotations = 0;
            Array.Copy(initialSymbols, symbols, symbols.Length);
        }

        public char EncryptSymbol(char s)
        {
            return symbols[s];
        }

        public char DecryptSymbol(char s)
        {
            for (int i = 0; i < symbols.Length; i++)
            {
                if (symbols[i] == s)
                {
                    s = (char)i;
                    break;
                }
            }
            return s;
        }

        public void Rotate()
        {
            if (numRotations == rotationsPeriod)
            {
                numRotations = 0;

                char c = symbols[0];
                Array.Copy(symbols, 1, symbols, 0, symbols.Length - 1);
                symbols[symbols.Length - 1] = c;
            }
            else
            {
                numRotations++;
            }
        }
    }

    class Reflector
    {
        private char[] symbols = null;

        public Reflector(int numSymbols)
        {
            symbols = new char[numSymbols];

            int h = numSymbols / 2;
            for (int i = 0; i < h; i++)
            {
                symbols[i] = (char)(h + i);
                symbols[h + i] = (char)i;
            }
        }

        public char EncryptSymbol(char s)
        {
            return symbols[s];
        }
    }

    class Enigma
    {
        private Rotor[] rotors = null;
        private Reflector reflector = null;

        private readonly int NUM_SYMBOLS = 256;

        public Enigma(Encoding encoding, int nRotors = 4)
        {
            if (encoding.EncodingName == Encoding.ASCII.EncodingName)
            {
                NUM_SYMBOLS = 256;
            }
            else if (encoding.EncodingName == Encoding.Unicode.EncodingName)
            {
                NUM_SYMBOLS = 65536;
            }

            rotors = new Rotor[nRotors];
            reflector = new Reflector(NUM_SYMBOLS);

            for (int i = 0; i < nRotors; i++)
            {
                rotors[i] = new Rotor(i * NUM_SYMBOLS + 1, NUM_SYMBOLS);
            }
        }

        public void Initialize()
        {
            foreach (var r in rotors)
            {
                r.Initialize();
            }
        }

        //EncryptByte(byte b)
        public char EncryptSymbol(char s)
        {
            for (int i = 0; i < rotors.Length; i++)
            {
                s = rotors[i].EncryptSymbol(s);
            }

            s = reflector.EncryptSymbol(s);

            for (int i = rotors.Length - 1; i >= 0; i--)
            {
                s = rotors[i].DecryptSymbol(s);
            }

            Rotate();

            return s;
        }

        //public string EncryptBytes(byte[] bytes)
        public string EncryptText(string text)
        {
            string eText = String.Empty;
            for (int i = 0; i < text.Length; i++)
            {
                eText += EncryptSymbol(text[i]);
            }
            return eText;
        }

        private void Rotate()
        {
            foreach (var r in rotors)
            {
                r.Rotate();
            }
        }
    }
}
