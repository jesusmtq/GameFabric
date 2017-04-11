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

namespace Salt.Processors
{
    [ReqestProcessor("UserExists", (int)RequestProcessorEnum.UserExists, typeof(GameFabric.Shared.Requests.UserExistsRequest), typeof(GameFabric.Shared.Responses.UserExistsResponse))]
    public class UserExistsProcessor: IRequestProcessor
    {
        public int ProcessorId
        {
            get
            {
                return ((int)RequestProcessorEnum.UserExists);
            }
        }

        public bool Authenticated
        {
            get { return (false); }
        }
        public async Task<IResponse> Process(IRequest request)
        {
            try
            {
                UserExistsRequest uer = request as UserExistsRequest;
                UserExistsResponse response = new UserExistsResponse();
                
                if (!string.IsNullOrEmpty(uer.UserName))
                {
                    //Compute actor id for user name
                    ActorId userid = uer.UserName.ToLowerInvariant().ToMD5GuidActorId();
                    var userproxy = userid.Proxy<IUser>();
                    bool exists = await userproxy.isCreatedAsync(); //Note: change to NOT create actors later, possible vector
                    response.Exists = exists;
                    response.Status = System.Net.HttpStatusCode.OK;
                }
                else
                {
                    response.Exists = false;
                    response.Status = System.Net.HttpStatusCode.NotAcceptable;
                }
                return (response);
            }
            catch (Exception E)
            {
                E.Log();
                ErrorResponse errorresponse= new ErrorResponse(E.Message);
                return (errorresponse);
            }
        }
    }
}
