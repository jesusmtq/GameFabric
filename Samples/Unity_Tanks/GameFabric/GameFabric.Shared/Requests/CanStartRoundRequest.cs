using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GameFabric.Processors.Interfaces;

namespace GameFabric.Shared.Requests
{
    public class CanStartRoundRequest : IRequest
    {
        public Guid GameSessionId { get; set; }
        public int RoundNo { get; set; }

        public CanStartRoundRequest()
        {

        }

        public CanStartRoundRequest(Guid SessionId,int Round)
        {
            GameSessionId = SessionId;
            RoundNo = Round;
        }
    }
}
