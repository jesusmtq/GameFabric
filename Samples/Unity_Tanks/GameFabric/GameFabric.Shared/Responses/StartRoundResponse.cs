using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GameFabric.Processors.Interfaces;
using System.Net;

namespace GameFabric.Shared.Responses
{
    public class StartRoundResponse : IResponse
    {
        public HttpStatusCode Status { get; set; }
        public Guid TankId { get; set; }
        public Guid UserId { get; set; }
        public int RoundNum { get; set; }

        public StartRoundResponse()
        {
            UserId = Guid.Empty;
            TankId = Guid.Empty;
            RoundNum = 0;
        }

        public StartRoundResponse(Guid aTankId, Guid aUserId,int round)
        {
            RoundNum = round;
            TankId = aTankId;
            UserId = aUserId;
        }
    }
}
