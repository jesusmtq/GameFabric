using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Actors;
using GameFabric.Common.Attributes;
using GameFabric.Models;
using GameFabric.Models.SystemModels;

namespace GameFabric.Interfaces
{
    [FabricLocation("fabric:/GameFabric.App/", "TankGameSessionActorService")]
    public interface ITankGameSession : IActor
    {
        Task<int> JoinAsync(ActorId UserId);
        Task<int> GetPlayerCountAsync(CancellationToken cancellationToken);
        Task<bool> CanStart(CancellationToken cancellationToken);
        Task<bool> StartSession(CancellationToken cancellationToken);
        Task<bool> TakeDamage(ActorId UserId, float amount);
        Task<bool> StartRound(ActorId aUserId);
        Task<bool> FinishRound();
        Task NotifyPosition(ActorId TankId, float x, float y, float z, float r);
        Task NotifyFireShell(ActorId TankId, string SerializedData);
        Task<List<TankSessionMemberItem>> GetPlayersAsync();
        Task<bool> CanStartRound(int roundNo);

    }
}
