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
    [ReqestProcessor("StartRound", (int)RequestProcessorEnum.StartRound, typeof(StartRoundRequest), typeof(StartRoundResponse))]
    public class StartRoundProcessor : IRequestProcessor
    {
        public int ProcessorId
        {
            get
            {
                return ((int)RequestProcessorEnum.StartRound);
            }
        }
        public bool Authenticated { get { return (false); } }
        public async Task<IResponse> Process(IRequest request)
        {
            try
            {
                StartRoundRequest rq = request as StartRoundRequest;
                StartRoundResponse response = new StartRoundResponse();
                //And notify
                ActorId SessionId = new ActorId(rq.TankGameSessionId);
                await SessionId.Proxy<ITankGameSession>().StartRound(new ActorId(rq.UserId));
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
