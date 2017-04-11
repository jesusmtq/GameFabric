using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GameFabric.Processors.Interfaces;
using System.Net;


namespace GameFabric.Shared.Responses
{
    public class CanStartRoundResponse : IResponse
    {
        public HttpStatusCode Status { get; set; }
        public Guid GameSessionId { get; set; }
        public bool CanStart { get; set; }
        public int RoundNum { get; set; }

        public CanStartRoundResponse()
        {

        }

        public CanStartRoundResponse(Guid GameSession,bool Start,int Round)
        {
            GameSessionId = GameSession;
            CanStart = Start;
            RoundNum = Round;
        }
    }
}
