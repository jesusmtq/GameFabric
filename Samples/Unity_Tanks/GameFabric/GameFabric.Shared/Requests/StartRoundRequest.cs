using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GameFabric.Processors.Interfaces;

namespace GameFabric.Shared.Requests
{

    public class StartRoundRequest : IRequest
    {
        public Guid TankGameSessionId { get; set; }
        public Guid TankId { get; set; }
        public Guid UserId { get; set; }

        public StartRoundRequest()
        {
            TankGameSessionId = Guid.Empty;
            TankId = Guid.Empty;
            UserId = Guid.Empty;
        }

        public StartRoundRequest(Guid SessionId, Guid aTankId,Guid aUserId)
        {
            TankGameSessionId = SessionId;
            TankId = aTankId;
            UserId = aUserId;
        }
    }
}
