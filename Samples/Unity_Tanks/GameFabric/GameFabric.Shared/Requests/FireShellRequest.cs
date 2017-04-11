using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GameFabric.Processors.Interfaces;

namespace GameFabric.Shared.Requests
{
    public class FireShellRequest : IRequest
    {
        public Guid TankGameSessionId { get; set; }
        public Guid TankId { get; set; }
        public clientVector3 pos { get; set; }
        public clientQuaternion rot { get; set; }
        public clientVector3 vel { get; set; }
        public FireShellRequest()
        {
            pos = new clientVector3();
            rot = new clientQuaternion();
            vel = new clientVector3();
        }

        public FireShellRequest(Guid aId, clientVector3 position, clientQuaternion rotation, clientVector3 velocity)
        {
            TankId = aId;
            pos = position;
            rot = rotation;
            vel = velocity;
        }
    }
    
}
