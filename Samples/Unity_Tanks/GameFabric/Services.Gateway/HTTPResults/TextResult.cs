using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Net.Http;
using System.Web.Http;

namespace GameFabric.Services.Gateway.HTTPResults
{
    /// <summary>
    /// Represents a textual httpactionresult
    /// </summary>
    public class TextResult : IHttpActionResult
    {
        #region local vars
        readonly string _Value;
        readonly HttpRequestMessage _Request;
        #endregion

        #region Creators
        public TextResult(string Value, HttpRequestMessage Request)
        {
            this._Value = Value;
            this._Request = Request;
        }
        #endregion

        #region Public methods
        public async Task<HttpResponseMessage> ExecuteAsync(CancellationToken cancellationToken)
        {
            HttpResponseMessage response = new HttpResponseMessage()
            {
                Content = new StringContent(_Value),
                RequestMessage = _Request
            };
            return await Task.FromResult(response);
        }
        #endregion
    }
}
