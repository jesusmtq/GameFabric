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
using GameFabric.Common.Hashes;
using GameFabric.Models.SystemModels;
using GameFabric.Logging;

namespace Actors.Session
{
    [StatePersistence(StatePersistence.Persisted)]
    internal class Session : Actor, ISession
    {

        public Session(ActorService actorService, ActorId actorId)
            : base(actorService, actorId)
        {
        }

        protected override Task OnActivateAsync()
        {
            ActorEventSource.Current.ActorMessage(this, "Actor activated.");
            return this.TryCreateStateAsync<SessionState>(new SessionState());
        }

        async Task<bool> ISession.CreateSessionAsync(ActorId UserActorId, string DeviceIdString)
        {
            try
            {
                SessionState state = await this.GetStateAsync<SessionState>();
                if (state.isCreated) return (false);
                //Try to create login state
                state.UserActorId = UserActorId;
                state.DeviceIdString = DeviceIdString;
                state.SessionKey = UserActorId.ToString() + this.Id.ToString();
                state.SessionHash = state.SessionKey.ToSHA256Hash();
                state.isCreated = true;
                await this.SetStateAsync(state);
                return (true);
            }
            catch (Exception E)
            {
                this.Log(E);
                return (false);
            }
        }

        async Task<string> ISession.GetSessionHashAsync()
        {
            try
            {
                SessionState state = await this.GetStateAsync<SessionState>();
                if (!state.isCreated) return (string.Empty);
                return (state.SessionHash);
            }
            catch (Exception E)
            {
                this.Log(E);
                return (string.Empty);
            }
        }

        async Task<bool> ISession.ValidateSessionAsync(ActorId UserActorId, string SessionHash)
        {
            try
            {
                SessionState state = await this.GetStateAsync<SessionState>();
                if (!state.isCreated) return (false);
                if (state.UserActorId != UserActorId) return (false);
                if (state.SessionHash != SessionHash) return (false);
                if (state.ValidTo < DateTime.UtcNow) return (false);
                return (true);
            }
            catch (Exception E)
            {
                this.Log(E);
                return (false);
            }
        }

        async Task ISession.SetUsedAsync()
        {
            SessionState state = await this.GetStateAsync<SessionState>();
            if (!state.isCreated) return;
            state.Used = DateTime.UtcNow;
            await this.SetStateAsync<SessionState>(state);
        }
    }
}
