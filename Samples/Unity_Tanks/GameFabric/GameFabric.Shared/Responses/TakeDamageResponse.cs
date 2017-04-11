using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GameFabric.Processors.Interfaces;
using System.Net;

namespace GameFabric.Shared.Responses
{
    public class TakeDamageResponse: IResponse
    {
        public HttpStatusCode Status { get; set; }
        public float Health { get; set; }
        public Guid TankId { get; set; }
        public Guid TankGameSessionId { get; set; }
        public bool isDead { get; set; }

        public TakeDamageResponse()
        {
            Status = System.Net.HttpStatusCode.OK;
        }
    }
}
