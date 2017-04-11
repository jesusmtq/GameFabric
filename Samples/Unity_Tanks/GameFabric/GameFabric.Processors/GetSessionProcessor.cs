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
using GameFabric.Logging;

namespace GameFabric.Processors
{
    [ReqestProcessor("CreateSession", (int)RequestProcessorEnum.Session, typeof(CreateSessionRequest),typeof(CreateSessionResponse))]
    public class GetSessionProcessor:IRequestProcessor
    {
        public int ProcessorId
        {
            get
            {
                return ((int)RequestProcessorEnum.Session);
            }
        }
        public bool Authenticated
        {
            get { return (true); }
        }
        public async Task<IResponse> Process(IRequest request)
        {
            try
            {
                CreateSessionRequest rq = request as CreateSessionRequest;
                CreateSessionResponse response = new CreateSessionResponse();
                if (rq.SessionKey==string.Empty) //noSession
                {
                    ActorId userActorId = new ActorId(rq.UserId);
                    var userProxy = userActorId.Proxy<IUser>();
                    if (await userProxy.isCreatedAsync())
                    {
                        var SessionProxy = userActorId.Proxy<ISession>();
                        if (await SessionProxy.CreateSessionAsync(userActorId, ""))
                        {
                            await SessionProxy.SetUsedAsync();
                            response.SessionKey = await SessionProxy.GetSessionHashAsync();
                            response.Status = System.Net.HttpStatusCode.OK;
                        }
                        else
                        {
                            await SessionProxy.SetUsedAsync();
                            response.SessionKey = await SessionProxy.GetSessionHashAsync();
                            response.Status = System.Net.HttpStatusCode.OK;
                        }
                    }
                    else
                    {
                        response.Status = System.Net.HttpStatusCode.Forbidden;
                        response.SessionKey = string.Empty;
                    }
                }
                else //Validate session
                {

                }
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
