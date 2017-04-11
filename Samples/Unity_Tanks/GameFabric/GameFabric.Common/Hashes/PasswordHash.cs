using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.IO;

namespace GameFabric.Common.Hashes
{
        /// <summary>
        ///     Class to provide salted password hashes to store user passwords secureley
        ///     Note that the salt is not encrypted
        /// </summary>
        public class PasswordHash
        {
            private const int SaltBytes = 32;
            public const int HashBytes = 32;

            /// <summary>
            /// Compute a salted Sha256 hash and output salt:hash as result (For storage of password)
            /// </summary>
            /// <param name="password">The password to use</param>
            /// <returns>salt:hash base64 string</returns>
            public string CreateSHA256PasswordHash(string password)
            {
                //Generate Salt
                var csp = new RNGCryptoServiceProvider();
                var salt = new byte[SaltBytes];
                csp.GetBytes(salt);

                //Get stream to sign
                var ms = new MemoryStream();
                ms.Write(salt, 0, SaltBytes);
                var bs = Encoding.UTF8.GetBytes(password);
                ms.Write(bs, 0, bs.Length);
                ms.Seek(0, SeekOrigin.Begin);

                //Generate hash
                var sha256P = new SHA256CryptoServiceProvider();
                var hash = sha256P.ComputeHash(ms);
                var toStore = Convert.ToBase64String(salt) + ":" + Convert.ToBase64String(hash);
                return toStore;
            }

            /// <summary>
            /// Validate a given password to a salt:hash signature
            /// </summary>
            /// <param name="password">Password to match</param>
            /// <param name="saltAndHash">salt:hash string</param>
            /// <returns>true/false</returns>
            public bool ValidateSHA256Password(string password, string saltAndHash)
            {
                try
                {
                    //Split hash
                    var segs = saltAndHash.Split(':').ToList();
                    var salt = Convert.FromBase64String(segs[0]);
                    var hash = Convert.FromBase64String(segs[1]);

                    //Recreate hash
                    var ms = new MemoryStream();
                    ms.Write(salt, 0, SaltBytes);
                    var bs = Encoding.UTF8.GetBytes(password);
                    ms.Write(bs, 0, bs.Length);
                    ms.Seek(0, SeekOrigin.Begin);

                    //Generate hash
                    var sha256P = new SHA256CryptoServiceProvider();
                    var newhash = sha256P.ComputeHash(ms);
                    var match = true;
                    for (var i = 0; i < hash.Length; i++)
                    {
                        if (hash[i] != newhash[i])
                        {
                            match = false;
                            break;
                        }
                    }
                    return match;
                }
                catch
                {
                    return false; //Always treat error as a negative match
                }
            }
        }
    }
