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
    [FabricLocation("fabric:/GameFabric.Gateway/", "GateActorService")]
    public interface IGate : IActor
    {
        Task<GateResponse> Process(GateRequest Request);
    }
}
