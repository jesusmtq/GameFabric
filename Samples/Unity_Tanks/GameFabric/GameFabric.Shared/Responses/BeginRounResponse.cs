using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GameFabric.Processors.Interfaces;
using System.Net;

namespace GameFabric.Shared.Responses
{
    /// <summary>
    /// Specific response without request or processor
    /// </summary>
    public class BeginRounResponse : IResponse
    {
        public HttpStatusCode Status { get; set; }
        public Guid GameSessionId { get; set; }
        public int RoundNum { get; set; }

        public BeginRounResponse()
        {
            GameSessionId = Guid.Empty;
            RoundNum = 0;
        }
        public BeginRounResponse(Guid aGameSessionId,int Round)
        {
            GameSessionId = aGameSessionId;
            RoundNum = Round;
        }
    }
}
