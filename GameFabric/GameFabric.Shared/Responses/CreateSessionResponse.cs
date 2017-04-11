using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GameFabric.Processors.Interfaces;
using System.Net;

namespace GameFabric.Shared.Responses
{
    public class CreateSessionResponse : IResponse
    {
        public HttpStatusCode Status { get; set; }
        public string SessionKey { get; set; }

        public CreateSessionResponse()
        {
            Status = HttpStatusCode.OK;
            SessionKey = string.Empty;
        }
    }
}
