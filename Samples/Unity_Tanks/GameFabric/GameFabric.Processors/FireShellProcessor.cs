using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameFabric.Common;
using GameFabric.Interfaces;
using GameFabric.Processors;
using GameFabric.Common.Attributes;
using GameFabric.Processors.Interfaces;
using GameFabric.Shared.Responses;
using GameFabric.Shared.Requests;
using Microsoft.ServiceFabric.Actors;
using GameFabric.Shared;
using System.Net;
using GameFabric.Logging;

namespace GameFabric.Processors
{
    [ReqestProcessor("FireShell", (int)RequestProcessorEnum.FireShell, typeof(FireShellRequest), typeof(FireShellResponse))]
    public class FireShellProcessor : IRequestProcessor
    {
        public int ProcessorId
        {
            get
            {
                return ((int)RequestProcessorEnum.FireShell);
            }
        }
        public bool Authenticated { get { return (false); } }
        public async Task<IResponse> Process(IRequest request)
        {
            try
            {
                FireShellRequest rq = request as FireShellRequest;
                FireShellResponse response = new FireShellResponse(rq.TankId,rq.pos,rq.rot,rq.vel);
                //And notify
                ActorId SessionId = new ActorId(rq.TankGameSessionId);
                await SessionId.Proxy<ITankGameSession>().NotifyFireShell(new ActorId(rq.TankId), response.Serialize());
                response.Status = HttpStatusCode.OK;
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
