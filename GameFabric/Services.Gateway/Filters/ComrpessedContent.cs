using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http.Filters;

namespace GameFabric.Services.Gateway.Filters
{
    public class CompressedContent : HttpContent
    {
        #region private vars
        private readonly string _encodingType;
        private readonly HttpContent _originalContent;
        #endregion
        #region Creators
        public CompressedContent(HttpContent Content, string encodingType = "gzip")
        {
            if (Content == null) throw new ArgumentNullException("Content");
            _originalContent = Content;
            _encodingType = encodingType.ToLowerInvariant();
            foreach (var header in _originalContent.Headers) this.Headers.TryAddWithoutValidation(header.Key, header.Value);
            this.Headers.ContentEncoding.Add(encodingType);
        }
        #endregion

        #region overrides
        protected override bool TryComputeLength(out long length)
        {
            length = -1;
            return false;
        }
        protected override Task SerializeToStreamAsync(Stream stream, TransportContext context)
        {
            Stream compressedStream = null;
            switch (_encodingType)
            {
                case "gzip":
                    compressedStream = new GZipStream(stream, CompressionMode.Compress, true);
                    break;
                case "deflate":
                    compressedStream = new DeflateStream(stream, CompressionMode.Compress, true);
                    break;
                default:
                    compressedStream = stream;
                    break;
            }
            return _originalContent.CopyToAsync(compressedStream).ContinueWith(tsk =>
            {
                if (compressedStream != null) compressedStream.Dispose();
            });
        }
        #endregion
    }


    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class CompressFilter : ActionFilterAttribute
    {
        public override void OnActionExecuted(HttpActionExecutedContext context)
        {
            string acceptedEncoding = context.Response.RequestMessage.Headers.AcceptEncoding.First().Value;
            if (!acceptedEncoding.Equals("gzip", StringComparison.InvariantCultureIgnoreCase)
            && !acceptedEncoding.Equals("deflate", StringComparison.InvariantCultureIgnoreCase))
            {
                return;
            }
            if (context.Response.Content != null) context.Response.Content = new CompressedContent(context.Response.Content, acceptedEncoding);
        }
    }
}
