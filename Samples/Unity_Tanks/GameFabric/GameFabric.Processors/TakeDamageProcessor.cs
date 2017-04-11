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
using GameFabric.Common.Hashes;
using System.Net;
using GameFabric.Models;
using GameFabric.Models.SystemModels;
using GameFabric.Logging;

namespace Salt.Processors
{
    [ReqestProcessor("TakeDamage", (int)RequestProcessorEnum.TakeDamage, typeof(TakeDamageRequest), typeof(TakeDamageResponse))]
    public class TakeDamageProcessor : IRequestProcessor
    {
        public int ProcessorId
        {
            get
            {
                return ((int)RequestProcessorEnum.TakeDamage);
            }
        }
        public bool Authenticated { get { return (false); } }
        public async Task<IResponse> Process(IRequest request)
        {
            try
            {
                TakeDamageRequest  rq = request as TakeDamageRequest;
                TakeDamageResponse response = new TakeDamageResponse();
                response.Status = HttpStatusCode.OK;
                response.TankGameSessionId = rq.TankGameSessionId;
                response.TankId = rq.TankId;

                //Do processing and notify clients
                ActorId SessionActorId = new ActorId(rq.TankGameSessionId);
                await SessionActorId.Proxy<ITankGameSession>().TakeDamage(new ActorId(rq.TankId), rq.Amount);
                //Send back response (dummy response here since we only support que in this call)
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
