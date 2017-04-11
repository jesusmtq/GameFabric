using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GameFabric.Processors.Interfaces;

namespace GameFabric.Shared.Requests
{
    public class TakeDamageRequest : IRequest
    {
        public Guid TankGameSessionId { get; set; }
        public float Amount { get; set; }
        public Guid TankId { get; set; }

        public TakeDamageRequest()
        {

        }
    }
}
