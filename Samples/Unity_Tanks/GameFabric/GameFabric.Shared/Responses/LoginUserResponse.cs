using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using GameFabric.Processors.Interfaces;

namespace GameFabric.Shared.Responses
{

    public class LoginUserResponse : IResponse
    {
        public HttpStatusCode Status { get; set; }
        public Guid UserId { get; set; }
        
        public LoginUserResponse()
        {
            Status = HttpStatusCode.OK;
            UserId = Guid.Empty;
        }
    }
}
