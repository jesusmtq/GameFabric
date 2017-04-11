using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using GameFabric.Common.Hashes;
using Microsoft.AspNet.SignalR.Infrastructure;

namespace GameFabric.SignalR
{

        /// <summary>
        /// Note this class is at current not optimized or supporting key configuration and statics in an acceptable manner.
        /// It does however provide multi-hos encryption/decryption withouth having to configure the same CurrentUser machine key
        /// on all host servers, thus allowing simple configuration and dynamic host scaling.
        /// </summary>
        public class FabricProtectedData : IProtectedData
        {
            private static readonly UTF8Encoding encoding = new UTF8Encoding(encoderShouldEmitUTF8Identifier: false, throwOnInvalidBytes: true);
            private static string password = "To replace with a real password";

            public string Protect(string data, string purpose)
            {
                //byte[] purposeBytes = _encoding.GetBytes(purpose);
                //byte[] unprotectedBytes = _encoding.GetBytes(data);
                //byte[] protectedBytes =  ProtectedData.Protect(unprotectedBytes, purposeBytes, DataProtectionScope.CurrentUser);
                //return Convert.ToBase64String(protectedBytes);
                return ToAESEncryptedString(data, password, purpose, password.ToMD5Hash().Substring(0, 16), 256);
            }

            public string Unprotect(string protectedValue, string purpose)
            {
                //byte[] purposeBytes = _encoding.GetBytes(purpose);
                //byte[] protectedBytes = Convert.FromBase64String(protectedValue);
                //byte[] unprotectedBytes =  ProtectedData.Unprotect(protectedBytes, purposeBytes, DataProtectionScope.CurrentUser);
                //return _encoding.GetString(unprotectedBytes);
                return FromAESEncryptedString(protectedValue, password, purpose, password.ToMD5Hash().Substring(0, 16), 256);
            }

            public string ToAESEncryptedString(string plainText, string password, string salt, string initialVector, int keySize)
            {
                var saltValueBytes = encoding.GetBytes(salt);
                var plainTextBytes = encoding.GetBytes(plainText);
                var initialVectorBytes = encoding.GetBytes(initialVector);
                var derivedPassword = new PasswordDeriveBytes(password, saltValueBytes, "MD5", 2);
                var keyBytes = derivedPassword.GetBytes(keySize / 8);

                var symmetricKey = new AesManaged();
                symmetricKey.Mode = CipherMode.CBC;

                using (var encryptor = symmetricKey.CreateEncryptor(keyBytes, initialVectorBytes))
                {
                    using (var stream = new MemoryStream())
                    {
                        using (var cryptoStream = new CryptoStream(stream, encryptor, CryptoStreamMode.Write))
                        {
                            cryptoStream.Write(plainTextBytes, 0, plainTextBytes.Length);
                            cryptoStream.FlushFinalBlock();

                            var cipherTextBytes = stream.ToArray();
                            return Convert.ToBase64String(cipherTextBytes);
                        }
                    }
                }
            }

            /// <summary>
            /// Decrypt an AES encrypted string using the parameters
            /// </summary>
            /// <param name="cipherText"></param>
            /// <param name="password"></param>
            /// <param name="salt"></param>
            /// <param name="HashAlgorithm"></param>
            /// <param name="PasswordIterations"></param>
            /// <param name="initialVector"></param>
            /// <param name="keySize"></param>
            /// <returns></returns>
            public string FromAESEncryptedString(string cipherText, string password, string salt, string initialVector, int keySize)
            {
                var initialVectorBytes = encoding.GetBytes(initialVector);
                var saltValueBytes = encoding.GetBytes(salt);
                var cipherTextBytes = Convert.FromBase64String(cipherText);
                var derivedPassword = new PasswordDeriveBytes(password, saltValueBytes, "MD5", 2);
                var keyBytes = derivedPassword.GetBytes(keySize / 8);

                var symmetricKey = new AesManaged();
                symmetricKey.Mode = CipherMode.CBC;

                using (var decryptor = symmetricKey.CreateDecryptor(keyBytes, initialVectorBytes))
                {
                    using (var stream = new MemoryStream(cipherTextBytes))
                    {
                        using (var cryptoStream = new CryptoStream(stream, decryptor, CryptoStreamMode.Read))
                        {
                            var plainTextBytes = new byte[cipherTextBytes.Length];
                            int byteCount = cryptoStream.Read(plainTextBytes, 0, plainTextBytes.Length);
                            return Encoding.UTF8.GetString(plainTextBytes, 0, byteCount);
                        }
                    }
                }
            }
        }
    }