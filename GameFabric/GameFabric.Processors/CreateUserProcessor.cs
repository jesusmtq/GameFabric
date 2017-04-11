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
    [ReqestProcessor("CreateUser", (int)RequestProcessorEnum.CreateUser, typeof(GameFabric.Shared.Requests.CreateUserRequest), typeof(GameFabric.Shared.Responses.CreateUserResponse))]
    public class CreateUserProcessor : IRequestProcessor
    {
        public int ProcessorId
        {
            get
            {
                return ((int)RequestProcessorEnum.CreateUser);
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
                CreateUserRequest uer = request as CreateUserRequest;
                CreateUserResponse response = new CreateUserResponse();

                if (!string.IsNullOrEmpty(uer.UserName) && !string.IsNullOrEmpty(uer.Password))
                {
                    //Compute actor id for user name
                    ActorId userid = uer.UserName.ToLowerInvariant().ToMD5GuidActorId();
                    var userproxy = userid.Proxy<IUser>();
                    bool exists = await userproxy.isCreatedAsync(); //Note: change to NOT create actors later, possible vector
                    if (!exists)
                    {
                        bool result = await userproxy.CreateUserAsync(uer.UserName, uer.Password, false);
                        response.Sucessful = true;
                        response.UserId = userid.GetGuidId();
                        response.Status = System.Net.HttpStatusCode.OK;
                    }
                    else
                    {
                        //Already exists
                        response.Sucessful = false;
                        response.UserId = Guid.Empty;
                        response.Status = System.Net.HttpStatusCode.Forbidden;
                    }
                    return (response);
                }
                else
                {
                    response.Sucessful = false;
                    response.UserId = Guid.Empty;
                    response.Status = System.Net.HttpStatusCode.BadRequest;
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
