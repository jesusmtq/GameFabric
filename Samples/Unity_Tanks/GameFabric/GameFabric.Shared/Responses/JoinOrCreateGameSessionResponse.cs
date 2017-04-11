using System;
using System.Collections.Generic;
using System.Linq;
using GameFabric.Processors.Interfaces;
using System.Net;


namespace GameFabric.Shared.Responses
{
    public class JoinOrCreateGameSessionResponse : IResponse
    {
        public HttpStatusCode Status { get; set; }
        public Guid GameSessionId { get; set; }
        public bool waitForPlayers { get; set; }
        public bool start { get; set; }
        public DateTime ValidTo { get; set; }
        public List<GameSessionPlayerItem> SessionPlayers { get; set; }
        public JoinOrCreateGameSessionResponse()
        {
            waitForPlayers = false;
            start = false;
            SessionPlayers = new List<Responses.GameSessionPlayerItem>();
        }
    }

    public class GameSessionPlayerItem
    {
        public Guid UserId { get; set; }
        public Guid TankId { get; set; }
        public int Sequence { get; set; }
        public GameSessionPlayerItem()
        {

        }

        public GameSessionPlayerItem(Guid aUser,Guid aTank,int aSequence)
        {
            UserId = aUser;
            TankId = aTank;
            Sequence = aSequence;
        }
    }
}
