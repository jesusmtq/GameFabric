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
    [ReqestProcessor("LoginUser", (int)RequestProcessorEnum.LoginUser, typeof(LoginUserRequest), typeof(LoginUserResponse))]
    public class LoginUserProcessor: IRequestProcessor
    {
        public int ProcessorId
        {
            get
            {
                return ((int)RequestProcessorEnum.LoginUser);
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
                LoginUserRequest lr = request as LoginUserRequest;
                LoginUserResponse response = new LoginUserResponse();

                if (!string.IsNullOrEmpty(lr.UserName) && !string.IsNullOrEmpty(lr.Password))
                {
                    //Compute actor id for user name
                    ActorId userid = lr.UserName.ToLowerInvariant().ToMD5GuidActorId();
                    var userproxy = userid.Proxy<IUser>();
                    bool exists = await userproxy.isCreatedAsync(); //Note: change to NOT create actors later, possible vector
                    if (exists)
                    {
                        var loginproxy = userid.Proxy<ILogin>();
                        if (await loginproxy.ValidatePassword(lr.Password))
                        {
                            response.UserId = userid.GetGuidId();
                            response.Status = System.Net.HttpStatusCode.OK;
                        }
                        else
                        {
                            response.UserId = Guid.Empty;
                            response.Status = System.Net.HttpStatusCode.Forbidden;
                        }
                    }
                    else
                    {
                        //Already exists
                        response.UserId = Guid.Empty;
                        response.Status = System.Net.HttpStatusCode.NotFound;
                    }
                    return (response);
                }
                else
                {
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
