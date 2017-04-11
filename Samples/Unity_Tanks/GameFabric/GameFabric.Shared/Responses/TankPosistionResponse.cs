using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GameFabric.Processors.Interfaces;
using System.Net;

namespace GameFabric.Shared.Responses
{

    public class TankPosistionResponse : IResponse
    {
        public HttpStatusCode Status { get; set; }
        public Guid TankId { get; set; }
        public float x { get; set; }
        public float y { get; set; }
        public float z { get; set; }
        public float r { get; set; }
        public TankPosistionResponse()
        {

        }

        public TankPosistionResponse(float xc, float yc, float zc,float rc)
        {
            x = xc;
            y = yc;
            z = zc;
            r = rc;
        }
        public TankPosistionResponse(Guid aTankId,float xc, float yc, float zc, float rc)
        {
            TankId = aTankId;
            x = xc;
            y = yc;
            z = zc;
            r = rc;
        }
    }
}
