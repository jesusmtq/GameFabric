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
    public class TankState
    {
        public ActorId OwningSessionId { get; set; }
        public float Health { get; set; }
        public TankState()
        {
            Health = 100.0f;
        }
        public TankState(ActorId OwningSession):this()
        {
            OwningSessionId = OwningSession;
        }
    }
}
