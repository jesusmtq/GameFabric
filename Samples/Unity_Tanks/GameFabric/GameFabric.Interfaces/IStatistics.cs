using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Actors;
using GameFabric.Models.SystemModels;
using GameFabric.Common.Attributes;

namespace GameFabric.Interfaces
{
    /// <summary>
    /// This interface defines the methods exposed by an actor.
    /// Clients use this interface to interact with the actor that implements it.
    /// </summary>
    [FabricLocation("fabric:/GameFabric.Data/", "StatisticsActorService")]
    public interface IStatistics : IActor
    {
        Task UpdateStats(string StatStorName,Dictionary<int, RequestStats> _Data);
        Task<List<RequestStats>> GetStatistics(string StatStorName);
        Task<string> GetStatisticsJSon(string StatStorName);
    }
}
