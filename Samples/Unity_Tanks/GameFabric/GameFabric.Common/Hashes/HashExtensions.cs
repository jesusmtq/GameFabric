using System;
using System.Globalization;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using Microsoft.ServiceFabric.Actors;

namespace GameFabric.Common.Hashes
{
    public static class HashExtensions
    {
        #region Password hash (AES)
        public static string ToPasswordHash(this string password)
        {
            var pwh = new PasswordHash();
            return pwh.CreateSHA256PasswordHash(password);
        }

        public static bool ValidatePasswordHash(this string password, string hash)
        {
            var pwh = new PasswordHash();
            return pwh.ValidateSHA256Password(password, hash);
        }
        #endregion

        #region Cryptographic hashes
        public static string ToMD5Hash(this string instring)
        {
            var md5p = new MD5CryptoServiceProvider();
            var bs = Encoding.UTF8.GetBytes(instring);
            bs = md5p.ComputeHash(bs);
            return Convert.ToBase64String(bs);
        }

        public static string ToSHA1Hash(this string instring)
        {
            var sha1p = new SHA1CryptoServiceProvider();
            var bs = Encoding.UTF8.GetBytes(instring);
            bs = sha1p.ComputeHash(bs);
            return Convert.ToBase64String(bs);
        }

        public static string ToSHA256Hash(this string instring)
        {
            var sha1p = new SHA256CryptoServiceProvider();
            var bs = Encoding.UTF8.GetBytes(instring);
            bs = sha1p.ComputeHash(bs);
            return Convert.ToBase64String(bs);
        }
        #endregion

        #region Guid tools
        public static Guid XorWith(this Guid g1, Guid g2)
        {
            var b1 = g1.ToByteArray();
            var b2 = g2.ToByteArray();
            for (var i = 0; i < 16; i++) b1[i] ^= b2[i];
            return new Guid(b1);
        }
        #endregion

        #region Cryptographic hashes - streaming
        public static string ToMD5Hash(this Stream instream)
        {
            var provider = new MD5CryptoServiceProvider();
            instream.Seek(0, SeekOrigin.Begin);
            var bytes = provider.ComputeHash(instream);
            return Convert.ToBase64String(bytes);
        }

        public static string ToSHA1Hash(this Stream instream)
        {
            var provider = new SHA1CryptoServiceProvider();
            instream.Seek(0, SeekOrigin.Begin);
            var bytes = provider.ComputeHash(instream);
            return Convert.ToBase64String(bytes);
        }
        #endregion

        #region SimpleCrypto
        public static string ToAESEncryptedString(this string plainText, string password)
        {
            var hash = password.ToMD5Hash().Substring(0, 16);
            var salt = hash.ToMD5Hash();
            return plainText.ToAESEncryptedString(password, salt, hash, 256);
        }

        public static string FromAESEncryptedString(this string cipherText, string password)
        {
            string hash = password.ToMD5Hash().Substring(0, 16);
            string Salt = hash.ToMD5Hash();
            return cipherText.FromAESEncryptedString(password, Salt, hash, 256);
        }

        public static string ToAESEncryptedString(this string plainText, string password, string salt, string initialVector, int keySize)
        {
            var saltValueBytes = Encoding.ASCII.GetBytes(salt);
            var plainTextBytes = Encoding.UTF8.GetBytes(plainText);
            var initialVectorBytes = Encoding.ASCII.GetBytes(initialVector);
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

        public static string FromAESEncryptedString(this string cipherText, string password, string salt, string initialVector, int keySize)
        {
            var initialVectorBytes = Encoding.ASCII.GetBytes(initialVector);
            var saltValueBytes = Encoding.ASCII.GetBytes(salt);
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
                        var byteCount = cryptoStream.Read(plainTextBytes, 0, plainTextBytes.Length);
                        return Encoding.UTF8.GetString(plainTextBytes, 0, byteCount);
                    }
                }
            }
        }
        #endregion

        #region Tools
        public static string ToMD5HashHex(this string instring)
        {
            var provider = new MD5CryptoServiceProvider();
            var bytes = Encoding.UTF8.GetBytes(instring);
            bytes = provider.ComputeHash(bytes);
            var result = bytes.ToHexString();
            return result;
        }

        public static Guid ToMD5AsGuid(this string instring)
        {
            var provider = new MD5CryptoServiceProvider();
            var bytes = Encoding.UTF8.GetBytes(instring);
            var hash = provider.ComputeHash(bytes);
            var hashGuid = new Guid(hash);
            return hashGuid;
        }

        public static Guid CombineGuids(this Guid first, Guid second)
        {
            var firstBytes = first.ToByteArray();
            var secondBytes = second.ToByteArray();
            for (var i = 0; i < firstBytes.Length; i++)
            {
                firstBytes[i] ^= secondBytes[i];
            }
            var result = new Guid(firstBytes);
            return result;
        }

        public static string ToHexString(this string instring)
        {
            return Encoding.UTF8.GetBytes(instring).ToHexString();
        }

        public static string ToHexString(this byte[] buffer)
        {
            var stringBuilder = new StringBuilder();
            char[] hexValues = { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'A', 'B', 'C', 'D', 'E', 'F' };
            foreach (byte b in buffer)
            {
                stringBuilder.Append(hexValues[b >> 4 & 0x0F]);
                stringBuilder.Append(hexValues[b & 0x0F]);
            }
            return stringBuilder.ToString();
        }

        public static byte[] FromHexString(this string hexstring)
        {
            if (string.IsNullOrEmpty(hexstring))
            {
                throw new ArgumentNullException();
            }

            if (hexstring.Length % 2 != 0)
            {
                throw new ArgumentOutOfRangeException();
            }

            var length = hexstring.Length / 2;
            var buffer = new byte[length];

            for (int i = 0; i < hexstring.Length; i += 2)
            {
                buffer[i / 2] = byte.Parse(hexstring.Substring(i, 2), NumberStyles.HexNumber);
            }

            return buffer;
        }
        #endregion

        #region ActorId helpers
        public static ActorId ToMD5GuidActorId(this string instring)
        {
            return new ActorId(instring.ToMD5AsGuid());
        }

        public static ActorId ToActorId(this Guid aGuid)
        {
            return (new ActorId(aGuid));
        }

        public static ActorId ToActorId(this string anId)
        {
            return anId.ToMD5GuidActorId();
        }

        #endregion
    }
}
