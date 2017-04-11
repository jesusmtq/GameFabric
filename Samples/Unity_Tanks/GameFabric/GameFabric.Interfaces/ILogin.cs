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
    [FabricLocation("fabric:/GameFabric.App/", "LoginActorService")]
    public interface ILogin : IActor
    {
        Task<bool> Create(ActorId owningUserId,string password);
        Task<bool> ValidatePassword(string password);
        Task<bool> SetPassword(string password);
        Task<bool> TryResetPassword(string oldPassword,string newPassword);
        Task<ActorId> GetUserActorId();
        Task SetUserActorId(ActorId UserActorId);
    }
}
