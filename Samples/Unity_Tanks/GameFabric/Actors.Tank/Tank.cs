using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Runtime;
using Microsoft.ServiceFabric.Actors.Client;
using GameFabric.Interfaces;
using GameFabric.Common;
using GameFabric.Models.SystemModels;
using GameFabric.Logging;
namespace Actors.Tank
{
    [StatePersistence(StatePersistence.Persisted)]
    internal class Tank : Actor, ITank
    {
        public Tank(ActorService actorService, ActorId actorId)
            : base(actorService, actorId)
        {
        }
        protected override Task OnActivateAsync()
        {
            ActorEventSource.Current.ActorMessage(this, "Actor activated.");
            return (this.SetStateAsync(new TankState()));
        }
        async Task<bool> ITank.SetTankGameSessionIdAsync(ActorId GameSessionId)
        {
            try
            {
                TankState s = await this.GetStateAsync<TankState>();
                s.OwningSessionId = GameSessionId;
                await this.SetStateAsync(s);
                return (true);
            }
            catch (Exception E)
            {
                this.Log(E);
                return (false);
            }
        }
        async Task<bool> ITank.isDeadAsync()
        {
            try
            {
                TankState s = await this.GetStateAsync<TankState>();
                if (s.Health <= 0) return (false);
                return (true);
            }
            catch (Exception E)
            {
                this.Log(E);
                return (false);
            }
        }
        async Task<float> ITank.TakeDamageAsync(float Amount)
        {
            try
            {
                TankState s = await this.GetStateAsync<TankState>();
                s.Health -= Amount;
                if (s.Health < 0) s.Health = 0;
                await this.SetStateAsync(s);
                return (s.Health);
            }
            catch (Exception E)
            {
                this.Log(E);
                return (0);
            }
        }

        async Task ITank.ResetAsync()
        {
            try
            {
                TankState s = new TankState(this.Id);
                await this.SetStateAsync<TankState>(s);
            }
            catch (Exception E)
            {
                this.Log(E);
            }
        }
    }
}
