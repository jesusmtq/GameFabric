using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GameFabric.Processors.Interfaces;
using System.Net;

namespace GameFabric.Shared.Responses
{
    public class UserExistsResponse : IResponse
    {
        public HttpStatusCode Status { get; set; }
        public bool Exists { get; set; }

        public UserExistsResponse()
        {
            Status = HttpStatusCode.OK;
            Exists=false;
        }
    }
}
