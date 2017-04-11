using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GameFabric.Processors.Interfaces;
using System.Net;

namespace GameFabric.Shared.Responses
{
    public class ErrorResponse : IResponse
    {
        public HttpStatusCode Status { get; set; }
        public string Message { get; set; }

        public ErrorResponse()
        {
            Status = HttpStatusCode.InternalServerError;
            Message = string.Empty;
        }

        public ErrorResponse(string aMessage):this()
        {
            Message = aMessage;
        }
    }
}
