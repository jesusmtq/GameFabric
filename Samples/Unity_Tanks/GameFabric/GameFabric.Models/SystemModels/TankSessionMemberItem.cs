using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Actors;
using System.Runtime.Serialization;
namespace GameFabric.Models.SystemModels
{
    [DataContract]
    public class TankSessionMemberItem
    {
        [DataMember]
        public ActorId UserId { get; set; }
        [DataMember]
        public ActorId TankId { get; set; }
        [DataMember]
        public int Sequence { get; set; }

        public int CurrentRound { get; set; }
        
        public TankSessionMemberItem()
        {
            TankId = ActorId.CreateRandom();
        }

        public TankSessionMemberItem(ActorId userId,int seq)
        {
            UserId = userId;
            TankId = new ActorId(Guid.NewGuid());//
            Sequence = seq;
            CurrentRound = 0;
        }

        public TankSessionMemberItem(ActorId userId,ActorId tankId)
        {
            UserId = userId;
            TankId = TankId;
            CurrentRound = 0;
        }

    }
}
