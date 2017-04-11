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
    [FabricLocation("fabric:/GameFabric.Data/", "GenericStorageActorService")]
    public interface IGenericStorage : IActor
    {
        Task<bool> ContainsDataAsync();
        Task<string> GetDataAsync();
        Task<bool> SetDataAsync(string Data);
        Task<bool> DeleteDataAsync();
        Task<DateTime> LastModifiedAsync();
        Task<DateTime> CreatedAsync();
    }
}
