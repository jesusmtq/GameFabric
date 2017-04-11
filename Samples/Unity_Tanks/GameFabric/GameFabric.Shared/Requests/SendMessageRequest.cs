using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GameFabric.Processors.Interfaces;

namespace GameFabric.Shared.Requests
{
    public class SendMessageRequest : IRequest
    {
        public Guid FromUserId { get; set; }
        public Guid ToUserId { get; set; }
        public string Message { get; set; }

        public SendMessageRequest()
        {
            FromUserId = Guid.Empty;
            ToUserId = Guid.Empty;
            Message = string.Empty;
        }
    }
}
