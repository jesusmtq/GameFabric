using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GameFabric.Processors.Interfaces;

namespace GameFabric.Shared.Requests
{
    public class CreateSessionRequest: IRequest
    {
        public Guid UserId { get; set; }
        public string SessionKey { get; set; }
    }
}
