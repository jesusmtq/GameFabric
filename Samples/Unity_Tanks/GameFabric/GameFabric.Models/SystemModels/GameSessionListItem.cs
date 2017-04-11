using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using Microsoft.ServiceFabric.Actors;

namespace GameFabric.Models.SystemModels
{
    [DataContract]
    public class GameSessionListItem
    {
        [DataMember]
        public ActorId GameSessionId { get; set; }
        [DataMember]
        public DateTime Created { get; set; }
        public DateTime ValidUntil { get; set; }
        public GameSessionListItem()
        {
            GameSessionId = null;
            Created = DateTime.UtcNow;
            ValidUntil = Created.AddMinutes(5);
        }

        public GameSessionListItem(ActorId SessionId):this()
        {
            GameSessionId = SessionId;
        }
    }
}
