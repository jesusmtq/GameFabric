using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.IO.Compression;

namespace GameFabric.Common.Compression
{
    public static class CompressionExtensions
    {
        public static string Compress(this string instring, bool useAscii = false)
        {
            if (!useAscii)
            {
                using (var memory = new MemoryStream())
                {
                    using (var gzip = new GZipStream(memory,
                        CompressionMode.Compress, true))
                    {
                        var data = Encoding.UTF8.GetBytes(instring);
                        gzip.Write(data, 0, data.Length);
                    }
                    return (Convert.ToBase64String(memory.ToArray()));
                }
            }
            else
            {
                using (var memory = new MemoryStream())
                {
                    using (var gzip = new GZipStream(memory,
                        CompressionMode.Compress, true))
                    {
                        var data = Encoding.ASCII.GetBytes(instring);
                        gzip.Write(data, 0, data.Length);
                    }
                    return (Convert.ToBase64String(memory.ToArray()));
                }
            }
        }

        public static string Decompress(this string instring, bool useAscii = false)
        {
            var data = Convert.FromBase64String(instring);
            if (!useAscii)
            {
                using (var stream = new GZipStream(new MemoryStream(data),
                    CompressionMode.Decompress))
                {
                    using (var memory = new MemoryStream())
                    {
                        stream.CopyTo(memory);
                        return (new string(Encoding.UTF8.GetChars(memory.ToArray())));
                    }
                }
            }
            else
            {
                using (var stream = new GZipStream(new MemoryStream(data),
                    CompressionMode.Decompress))
                {
                    using (var memory = new MemoryStream())
                    {
                        stream.CopyTo(memory);
                        return (new string(Encoding.ASCII.GetChars(memory.ToArray())));
                    }
                }
            }
        }
    }
}
