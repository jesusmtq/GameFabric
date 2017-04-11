using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using System.Threading.Tasks;
using GameFabric.Interfaces;
using GameFabric.Common;
using GameFabric.Common.Hashes;
using GameFabric.Services.Gateway.Hubs;
using Microsoft.ServiceFabric.Actors.Client;
using GameFabric.Logging;

namespace GameFabric.Services.Gateway.Support
{
    public class SignalRUserMapper
    {
        #region private vars
        private ConnectionLookup connLookup;
        private ConcurrentDictionary<Guid, ActorEventHandler> eventHandlerLookup;
        private IHubConnectionContext<dynamic> hubClients;
        private ConcurrentDictionary<Guid, IUser> proxyLookup;
        #endregion


        #region Singleton
        public static SignalRUserMapper Instance
        {
            get
            {

                return (SRCore.instance);
            }
        }
        #region internal core
        class SRCore
        {
            static SRCore()
            {
            }

            internal static readonly SignalRUserMapper instance = new SignalRUserMapper();
        }
        #endregion
        #endregion

        private SignalRUserMapper()
        {
            Init();
        }

        public int NumOnline => connLookup.NumUsers;
        public int NumActiveConnections => connLookup.NumConnections;

        private void Init()
        {
            connLookup = new ConnectionLookup();
            proxyLookup = new ConcurrentDictionary<Guid, IUser>();
            eventHandlerLookup = new ConcurrentDictionary<Guid, ActorEventHandler>();
            hubClients = GlobalHost.ConnectionManager.GetHubContext<InterfaceHub>().Clients;
        }

        public void MapContext(IHubConnectionContext<dynamic> ctx)
        {
            hubClients = ctx;
        }

        public async Task<bool> AddUserMapping(string userId, string connectionId)
        {
            var result = false;
            try
            {
                Guid guidUserId = new Guid(userId);
                if (!proxyLookup.ContainsKey(guidUserId))
                {
                    var eventHandler = new ActorEventHandler(userId);
                    proxyLookup.TryAdd(guidUserId, guidUserId.ToActorId().Proxy<IUser>());
                    eventHandlerLookup.TryAdd(guidUserId, eventHandler);
                    await proxyLookup[guidUserId].SubscribeAsync(eventHandler,TimeSpan.FromSeconds(30));
                }
                result = connLookup.AddConnection(userId, connectionId);
            }
            catch (Exception e)
            {
                e.Log();
            }
            return result;
        }

        public async Task<bool> RemoveUserMappingByConnectionId(string connectionId)
        {
            var result = false;
            try
            {
                var guidUserId = new Guid(connLookup.GetUserFromConnection(connectionId));
                if (proxyLookup.ContainsKey(guidUserId))
                {
                    var proxy = proxyLookup[guidUserId];
                    ActorEventHandler evenHandler = eventHandlerLookup[guidUserId];
                    await proxy.UnsubscribeAsync<IUserEvent>(evenHandler);

                    IUser removedActor;
                    ActorEventHandler removedEventHandler;
                    proxyLookup.TryRemove(guidUserId, out removedActor);
                    eventHandlerLookup.TryRemove(guidUserId, out removedEventHandler);
                }
                result = connLookup.RemoveConnection(connectionId);
            }
            catch (Exception e)
            {
                e.Log();
            }
            return result;
        }

        public async Task RemoveUserMappingByUserId(string userId)
        {
            List<string> connectionIds = GetConnectionIds(userId);
            foreach (string id in connectionIds)
            {
                connLookup.RemoveConnection(id);
            }
            await Task.FromResult(true);
        }

        public List<string> GetConnectionIds(string userId)
        {
            return connLookup.GetUserConnections(userId);
        }

        public string GetUserId(string connectionId)
        {
            return connLookup.GetUserFromConnection(connectionId);
        }

        public async Task SendEventByUserId(string userId, string message)
        {
            if (string.IsNullOrEmpty(message) || string.IsNullOrEmpty(userId)) return;
            var conids = GetConnectionIds(userId);
            if (conids.Count > 0)
            {
                try
                {
                    IClientProxy proxy = hubClients.Clients(conids);
                    await proxy.Invoke("Exec", message);
                }
                catch (Exception e)
                {
                    e.Log();
                }
            }
            else
            {
                $"Failed to send event to {userId}, no connections".Log();
            }
        }

        public async Task SendQueueResponseByUserId(string userId, string message)
        {
            if (string.IsNullOrEmpty(message) || string.IsNullOrEmpty(userId)) return;
            var conids = GetConnectionIds(userId);

            if (conids.Count > 0)
            {
                IClientProxy proxy = hubClients.Clients(conids);
                await proxy.Invoke("Exec", message);
            }
        }
    }
}
