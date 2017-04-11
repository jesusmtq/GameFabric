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
    [FabricLocation("fabric:/GameFabric.App/", "SessionActorService")]
    public interface ISession : IActor
    {
        Task<bool> CreateSessionAsync(ActorId UserActorId, string DeviceIdString);
        Task<string> GetSessionHashAsync();
        Task<bool> ValidateSessionAsync(ActorId UserActorId, string SessionHash);
        Task SetUsedAsync();
    }
}
