using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameFabric.Common;
using GameFabric.Interfaces;
using GameFabric.Processors;
using GameFabric.Models;
using GameFabric.Common.Attributes;
using GameFabric.Processors.Interfaces;
using GameFabric.Shared.Responses;
using GameFabric.Shared.Requests;
using Microsoft.ServiceFabric.Actors;
using System.Net;
using GameFabric.Logging;

namespace GameFabric.Processors
{
    [ReqestProcessor("TankPosition", (int)RequestProcessorEnum.TankPosition, typeof(TankPositionQueRequest), typeof(TankPosistionResponse))]
    public class TankPositionProcessor: IRequestProcessor
    {
        public int ProcessorId {get{return ((int)RequestProcessorEnum.TankPosition);}}
        public bool Authenticated {get { return (false); }}
        public async Task<IResponse> Process(IRequest request)
        {
            try
            {
                TankPositionQueRequest rq = request as TankPositionQueRequest;
                ActorId SessionId = new ActorId(rq.TankGameSessionId);
                await SessionId.Proxy<ITankGameSession>().NotifyPosition(new ActorId(rq.TankId), rq.x, rq.y, rq.z, rq.r);
                //Do que resolution
                TankPosistionResponse response = new TankPosistionResponse(rq.x,rq.y,rq.z,rq.r);
                response.TankId = rq.TankId;
                return (await Task.FromResult(response));
            }
            catch (Exception E)
            {
                E.Log();
                ErrorResponse errorresponse = new ErrorResponse(E.Message);
                return (errorresponse);
            }
        }
    }
}
