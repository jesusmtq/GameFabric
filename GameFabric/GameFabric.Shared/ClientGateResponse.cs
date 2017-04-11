using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GameFabric.Shared
{
    public class ClientGateResponse
    {
        #region Creators
        public ClientGateResponse()
        {

        }

        public ClientGateResponse(ClientGateRequest CreateForRequest)
        {
            this.Id = CreateForRequest.Id;
            this.Created = CreateForRequest.Created;
            this.Kind = CreateForRequest.Kind;
        }
        #endregion

        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public int Kind { get; set; }
        public string JsonPayload { get; set; }
        public bool isCompressed { get; set; }
        public int ResultCode { get; set; }
        public long Created { get; set; }
        public long TimeTaken { get; set; }
    }
}
