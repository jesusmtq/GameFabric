using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Actors;
using GameFabric.Common.Attributes;
using GameFabric.Models;

namespace GameFabric.Interfaces
{
    /// <summary>
    /// This interface defines the methods exposed by an actor.
    /// Clients use this interface to interact with the actor that implements it.
    /// </summary>
    [FabricLocation("fabric:/GameFabric.App/", "UserActorService")]
    public interface IUser : IActor, IActorEventPublisher<IUserEvent>
    {
        Task<bool> CreateUserAsync(string UserName, string Password, bool isDeveloper);
        Task<bool> isCreatedAsync();
        Task<bool> SendNotificationAsync(int kind, string payload);
        Task<bool> SendGateResponseAsync(GateResponse response);
    }
}
