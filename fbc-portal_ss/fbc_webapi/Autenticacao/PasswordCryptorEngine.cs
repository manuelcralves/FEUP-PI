using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace fbc_webapi.Autenticacao
{
    internal static class PasswordCryptorEngine
    {
        private static readonly string Key = "Q~4iYSyQ*A[R@3r',j._NPr:tédVdJ+";
        private static readonly HashAlgorithmName HashAlgorithm = HashAlgorithmName.SHA512;
        private static readonly int IterationsCount = 500;
        private static readonly int SaltSize = 8;
        private static readonly int KeySize = 32;
        private static readonly int IVSize = 16;

        public static byte[] Encrypt(string plainTextValue)
        {
            byte[] plainTextBytes = Encoding.UTF8.GetBytes(plainTextValue);

            byte[] resultArray;
            using (Rfc2898DeriveBytes keyGenerator = new Rfc2898DeriveBytes(Key, SaltSize, IterationsCount, HashAlgorithm))
            {
                using (Aes aesCSP = Aes.Create())
                {
                    aesCSP.Key = keyGenerator.GetBytes(KeySize);
                    aesCSP.IV = keyGenerator.GetBytes(IVSize);
                    aesCSP.Mode = CipherMode.CBC;
                    aesCSP.Padding = PaddingMode.PKCS7;

                    using (ICryptoTransform cTransform = aesCSP.CreateEncryptor())
                        resultArray = cTransform.TransformFinalBlock(plainTextBytes, 0, plainTextBytes.Length);
                }

                resultArray = Enumerable.Concat(keyGenerator.Salt, resultArray).ToArray();
            }

            return resultArray;
        }

        public static string Decrypt(byte[] toDecryptArray)
        {
            byte[] salt = new byte[SaltSize];
            Array.Copy(toDecryptArray, 0, salt, 0, salt.Length);

            byte[] resultArray;
            using (Rfc2898DeriveBytes keyGenerator = new Rfc2898DeriveBytes(Key, salt, IterationsCount, HashAlgorithm))
            {
                using (Aes aesCSP = Aes.Create())
                {
                    aesCSP.Key = keyGenerator.GetBytes(KeySize);
                    aesCSP.IV = keyGenerator.GetBytes(IVSize);
                    aesCSP.Mode = CipherMode.CBC;
                    aesCSP.Padding = PaddingMode.PKCS7;

                    using (ICryptoTransform cTransform = aesCSP.CreateDecryptor())
                        resultArray = cTransform.TransformFinalBlock(toDecryptArray, salt.Length, toDecryptArray.Length - salt.Length);
                }
            }

            return Encoding.UTF8.GetString(resultArray, 0, resultArray.Length);
        }
    }
}