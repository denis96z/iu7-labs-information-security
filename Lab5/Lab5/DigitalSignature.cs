using System;
using System.IO;
using System.Security.Cryptography;

namespace Lab5
{
    class DigitalSignature
    {
        private const string HASH_ALG_NAME = "SHA1";

        public void CreateSignatureFile(string sourceFileName, string signatureFileName)
        {
            var hashAlgorithm = HashAlgorithm.Create(HASH_ALG_NAME);
            var cryptoServiceProvider = new DSACryptoServiceProvider();

            var signatureFormatter = new DSASignatureFormatter(cryptoServiceProvider);
            signatureFormatter.SetHashAlgorithm(HASH_ALG_NAME);

            var cspBlob = cryptoServiceProvider.ExportCspBlob(false);

            var signFileStream = File.Create(signatureFileName);
            using (var reader = File.OpenRead(sourceFileName))
            using (var writer = new BinaryWriter(signFileStream))
            {
                var hash = hashAlgorithm.ComputeHash(reader);
                var signature = signatureFormatter.CreateSignature(hash);

                writer.Write(cspBlob);
                writer.Write(signature);

                reader.Close();
                writer.Close();
            }

            hashAlgorithm.Dispose();
            cryptoServiceProvider.Dispose();
        }

        public bool VerifySignature(string sourceFileName, string signatureFileName)
        {
            var hashAlgorithm = HashAlgorithm.Create(HASH_ALG_NAME);
            var cryptoServiceProvider = new DSACryptoServiceProvider();

            var signatureDeformatter = new DSASignatureDeformatter(cryptoServiceProvider);
            signatureDeformatter.SetHashAlgorithm(HASH_ALG_NAME);

            bool result = false;

            var signatureStream = File.OpenRead(signatureFileName);
            using (var sourceReader = File.OpenRead(sourceFileName))
            using (var signatureReader = new BinaryReader(signatureStream))
            {
                var cspBlobLength = cryptoServiceProvider
                    .ExportCspBlob(false).Length;
                var cspBlob = signatureReader.ReadBytes(cspBlobLength);
                cryptoServiceProvider.ImportCspBlob(cspBlob);

                var hash = hashAlgorithm.ComputeHash(sourceReader);
                var signatureLength = (int)(signatureReader.BaseStream.Length -
                    signatureReader.BaseStream.Position);
                var signature = signatureReader.ReadBytes(signatureLength);

                result = signatureDeformatter.VerifySignature(hash, signature);

                sourceReader.Close();
                signatureReader.Close();
            }

            hashAlgorithm.Dispose();
            cryptoServiceProvider.Dispose();

            return result;
        }
    }
}
