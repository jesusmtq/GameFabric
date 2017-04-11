using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GameFabric.Processors.Interfaces;
using System.Net;

namespace GameFabric.Shared.Responses
{
    public class CreateUserResponse : IResponse
    {
        public HttpStatusCode Status { get; set; }
        public bool Sucessful { get; set; }
        public Guid UserId { get; set; }

        public CreateUserResponse()
        {
            Status = HttpStatusCode.OK;
            Sucessful = false;
        }
    }
}
