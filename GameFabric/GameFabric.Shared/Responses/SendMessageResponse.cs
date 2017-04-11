using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GameFabric.Processors.Interfaces;
using System.Net;

namespace GameFabric.Shared.Responses
{
    public class SendMessageResponse : IResponse
    {
        public HttpStatusCode Status { get; set; }
        public Guid FromUserId { get; set; }
        public string Message { get; set; }

        public SendMessageResponse()
        {
            FromUserId = Guid.Empty;
            Message = string.Empty;
        }
    }
}
