using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR;
using GameFabric.Services.Gateway.Support;
using GameFabric.Common;
using Microsoft.ServiceFabric.Actors;
using GameFabric.Interfaces;
using GameFabric.Common.Hashes;
using GameFabric.Models;
using GameFabric.Processors;
using GameFabric.Models.SystemModels;
using GameFabric.Logging;

namespace GameFabric.Services.Gateway.Hubs
{
    /// <summary>
    /// This is the main hub used to access the ServiceFabric backend using SignalR
    /// </summary>
    [Microsoft.AspNet.SignalR.Hubs.HubName("InterfaceHub")]
    public class InterfaceHub: Hub
    {
        #region counters
        /// <summary>
        /// Tracks number of recieved exec calls
        /// </summary>
        public static int _NumCalls = 0;
        /// <summary>
        /// Tracks number of recieved Que calls
        /// </summary>
        public static int _NumQues = 0;
        /// <summary>
        /// Tracks the number of recieved UserId->ConnectionId mappings and subscriptions
        /// </summary>
        public static int _MapCount = 0;
        #endregion

        /// <summary>
        /// Main Request-direct response method, will process a client request and directly invoke the calling client when done
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        [Microsoft.AspNet.SignalR.Hubs.HubMethodName("Exec")]
        public async Task Execute(string s)
        {
            //Use the current connection id (Guid) as the identity of the stateless gateway actor
            ActorId gatewayId = new ActorId(Context.ConnectionId);
            try
            {
                //Increment counter
                Interlocked.Increment(ref InterfaceHub._NumCalls);
                //Deserialize envelope request
                GateRequest request = s.Deserialize<GateRequest>();
                //call ServiceFabric gateway actor and process the request
                GateResponse response= await gatewayId.Proxy<IGate>().Process(request);
                //Return the response to the caller
                await Clients.Caller.Invoke("Exec",response.Serialize());
            }
            catch (Exception e)
            {
                e.Log();
            }
        }

        /// <summary>
        /// This method performs a mapping (once the client is logged in) that maps a connection id to a user id, then sets up subscriptions to ServiceFabric
        /// that can be invoked trough the UserActor and send messages back to the connected and mapped client
        /// </summary>
        /// <param name="uid"></param>
        /// <returns></returns>
        [Microsoft.AspNet.SignalR.Hubs.HubMethodName("Map")]
        public async Task Map(string uid)
        {
            try
            {
                Interlocked.Increment(ref _MapCount);
                string cId = Context.ConnectionId;
                if (string.IsNullOrEmpty(uid))
                {
                    throw new Exception("Invalid userId provided");
                }
                await SignalRUserMapper.Instance.RemoveUserMappingByConnectionId(cId);
                await SignalRUserMapper.Instance.AddUserMapping(uid, cId);
                //Callback to the client if any steps are needed post-map at the client
                await Clients.Caller.Invoke("Map", Context.ConnectionId);
            }
            catch (Exception e)
            {
                e.Log();
            }
        }
        /// <summary>
        /// This is the main "que" method - it sends a request to the gateway actor and then exits, any responses (if any) will be returned to the caller 
        /// (or multiple targets) using the subscription model.
        /// </summary>
        /// <param name="uid"></param>
        /// <returns></returns>
        [Microsoft.AspNet.SignalR.Hubs.HubMethodName("Que")]
        public async Task ExecuteAsync(string s)
        {
            Interlocked.Increment(ref InterfaceHub._NumQues);
            ActorId gatewayId = new ActorId(Context.ConnectionId);
            try
            {
                GateRequest request = s.Deserialize<GateRequest>();
                GateResponse response = await gatewayId.Proxy<IGate>().Process(request);
            }
            catch (Exception e)
            {
                e.Log();
            }
        }

        #region ConnectionHandling
        public override async Task OnConnected()
        {
            string connectionId = Context.ConnectionId;
            $"Connection {Context.ConnectionId} connected".LogDebug();
            await SignalRUserMapper.Instance.RemoveUserMappingByConnectionId(connectionId);
            await base.OnConnected();
        }
        public override async Task OnDisconnected(bool stopCalled)
        {
            var connectionId = Context.ConnectionId;
            $"Connection {Context.ConnectionId} disconnected".LogDebug();
            await SignalRUserMapper.Instance.RemoveUserMappingByConnectionId(connectionId);
            await base.OnDisconnected(stopCalled);
        }
        public override async Task OnReconnected()
        {
            var connectionId = Context.ConnectionId;
            $"Connection {Context.ConnectionId} reconnected".LogDebug();
            await SignalRUserMapper.Instance.RemoveUserMappingByConnectionId(connectionId);
            await base.OnReconnected();
        }
        #endregion
    }
}
