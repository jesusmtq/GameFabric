using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GameFabric.Processors.Interfaces;

namespace GameFabric.Shared.Requests
{
    public class TankPositionQueRequest : IRequest
    {
        public Guid TankGameSessionId { get; set; }
        public Guid TankId { get; set; }
        public float x { get; set; }
        public float y { get; set; }
        public float z { get; set; }
        public float r { get; set; }

        public TankPositionQueRequest()
        {

        }

        public TankPositionQueRequest(float xc,float yc,float zc,float rc)
        {
            x = xc;
            y = yc;
            z = zc;
            r = rc;
        }
        public TankPositionQueRequest(Guid aTankGameSessionId, Guid aTankId, float xc, float yc, float zc, float rc)
        {
            TankGameSessionId = aTankGameSessionId;
            TankId = aTankId;
            x = xc;
            y = yc;
            z = zc;
            r = rc;
        }
    }

}
