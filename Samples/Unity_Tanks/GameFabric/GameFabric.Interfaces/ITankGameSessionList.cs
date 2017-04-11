using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Actors;
using GameFabric.Models;
using GameFabric.Common.Attributes;
using GameFabric.Models.SystemModels;

namespace GameFabric.Interfaces
{
    /// <summary>
    /// This interface defines the methods exposed by an actor.
    /// Clients use this interface to interact with the actor that implements it.
    /// </summary>
    [FabricLocation("fabric:/GameFabric.App/", "TankGameSessionListActorService")]
    public interface ITankGameSessionList : IActor
    {

        Task<bool> AddGameSessionAsync(ActorId GameSessionId);
        Task<GameSessionListItem> GetNextSessionAsync();
        Task<bool> RemoveGameSessionAsync(ActorId GameSessionId);
        Task<bool> HasSessionsAsync();
    }
}
