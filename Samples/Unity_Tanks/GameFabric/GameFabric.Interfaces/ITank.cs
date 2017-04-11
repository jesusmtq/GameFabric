using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Actors;
using GameFabric.Common.Attributes;


namespace GameFabric.Interfaces
{
    /// <summary>
    /// This interface defines the methods exposed by an actor.
    /// Clients use this interface to interact with the actor that implements it.
    /// </summary>
    [FabricLocation("fabric:/GameFabric.App/", "TankActorService")]
    public interface ITank : IActor
    {
        Task<bool> SetTankGameSessionIdAsync(ActorId GameSessionId);
        Task<bool> isDeadAsync();
        Task<float> TakeDamageAsync(float Amount);
        Task ResetAsync();
    }
}
