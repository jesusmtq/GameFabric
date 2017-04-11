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
using GameFabric.Logging;

namespace GameFabric.Processors
{
    [ReqestProcessor("CanStartRound", (int)RequestProcessorEnum.GetCanStartRound, typeof(GameFabric.Shared.Requests.CanStartRoundRequest), typeof(GameFabric.Shared.Responses.CanStartRoundResponse))]
    public class CanStartRoundProcessor : IRequestProcessor
    {
        public int ProcessorId
        {
            get
            {
                return ((int)RequestProcessorEnum.GetCanStartRound);
            }
        }
        public bool Authenticated { get { return (false); } }
        public async Task<IResponse> Process(IRequest request)
        {
            try
            {
                CanStartRoundRequest csr = request as CanStartRoundRequest;
                CanStartRoundResponse response = new CanStartRoundResponse();

                if (csr.GameSessionId!=Guid.Empty)
                {
                    //Compute actor id for user name
                    ActorId gamesessionid = new ActorId(csr.GameSessionId);
                    var sessionproxy = gamesessionid.Proxy<ITankGameSession>();
                    bool canstart = await sessionproxy.CanStartRound(csr.RoundNo);
                    response = new CanStartRoundResponse(csr.GameSessionId, canstart,csr.RoundNo);
                    return (response);
                }
                return (response);
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
