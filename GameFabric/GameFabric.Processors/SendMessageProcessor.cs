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
    [ReqestProcessor("SendMessage", (int)RequestProcessorEnum.SendMessage, typeof(SendMessageRequest), typeof(SendMessageResponse))]
    public class SendMessageProcessor : IRequestProcessor
    {
        public int ProcessorId
        {
            get
            {
                return ((int)RequestProcessorEnum.SendMessage);
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
                SendMessageRequest sendRequest = request as SendMessageRequest;
                SendMessageResponse response = new SendMessageResponse();
                EmptyResponse empty = new EmptyResponse();

                if (sendRequest.FromUserId!=Guid.Empty && sendRequest.ToUserId!=Guid.Empty)
                {
                    //Test reciever exists
                    ActorId userId = new ActorId(sendRequest.ToUserId);
                    var userproxy = userId.Proxy<IUser>();
                    bool exists = await userproxy.isCreatedAsync();
                    if (exists)
                    {
                        response.FromUserId = sendRequest.FromUserId;
                        response.Message = sendRequest.Message;
                        await userproxy.SendGateResponseAsync(new Models.GateResponse(this.ProcessorId, (int)System.Net.HttpStatusCode.OK, sendRequest.ToUserId, response.Serialize()));
                    }
                    else
                    {
                        empty.Status = System.Net.HttpStatusCode.BadRequest;
                    }
                    return (empty);
                }
                else
                {
                    empty.Status = System.Net.HttpStatusCode.BadRequest;
                }
                return (empty);
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
