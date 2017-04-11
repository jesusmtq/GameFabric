using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GameFabric.Processors.Interfaces;
using System.Net;

namespace GameFabric.Shared.Responses
{
    public class EmptyResponse : IResponse
    {
        public HttpStatusCode Status { get; set; }
        public EmptyResponse()
        {
            Status = HttpStatusCode.OK;
        }
    }
}
