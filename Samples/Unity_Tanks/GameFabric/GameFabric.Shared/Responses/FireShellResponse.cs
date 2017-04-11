using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GameFabric.Processors.Interfaces;
using System.Net;

namespace GameFabric.Shared.Responses
{
    
    public class FireShellResponse : IResponse
    {
        public HttpStatusCode Status { get; set; }
        public Guid TankId { get; set; }
        public clientVector3 pos { get; set; }
        public clientQuaternion rot { get; set; }
        public clientVector3 vel { get; set; }
        public FireShellResponse()
        {
            Status = HttpStatusCode.OK;
            pos = new clientVector3();
            rot = new clientQuaternion();
            vel = new clientVector3();
        }

        public FireShellResponse(Guid aId, clientVector3 position, clientQuaternion rotation, clientVector3 velocity)
        {
            TankId = aId;
            Status = HttpStatusCode.OK;
            pos = position;
            rot = rotation;
            vel = velocity;
        }
    }
}
