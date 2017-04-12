using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Runtime;
using Microsoft.ServiceFabric.Actors.Client;
using GameFabric.Interfaces;
using GameFabric.Models;
using GameFabric.Common;
using GameFabric.Common.Hashes;
using GameFabric.Models.SystemModels;
using GameFabric.Logging;

namespace Actors.User
{
    [StatePersistence(StatePersistence.Persisted)]
    internal class User : Actor, IUser
    {

        public User(ActorService actorService, ActorId actorId)
            : base(actorService, actorId)
        {
        }
        protected override Task OnActivateAsync()
        {
            ActorEventSource.Current.ActorMessage(this, "Actor activated.");
            return this.TryCreateStateAsync<UserState>(new UserState());
        }
        async Task<bool> IUser.CreateUserAsync(string UserName, string Password, bool isDeveloper)
        {
            try
            {
                UserState state = await this.GetStateAsync<UserState>();
                if (state.isCreated) return (false);
                //Try to create login state
                ActorId loginId = UserName.ToLowerInvariant().ToMD5GuidActorId();
                var loginproxy = loginId.Proxy<ILogin>();
                if (!await loginproxy.Create(this.Id, Password)) return (false);
                //Set rest of user state
                state.LoginActorId = loginId;
                state.UserName = UserName;
                state.isDeveloper = isDeveloper;
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
        async Task<bool> IUser.isCreatedAsync()
        {
            try
            {
                UserState state = await this.GetStateAsync<UserState>();
                return (state.isCreated);
            }
            catch (Exception E)
            {
                this.Log(E);
                return (false);
            }
        }

        async Task<bool> IUser.SendGateResponseAsync(GateResponse response)
        {
            try
            {
                UserState state = await this.GetStateAsync<UserState>();
                if (!state.isCreated) return (false);
                var ev = GetEvent<IUserEvent>();
                ev.SendGateResponseAsync(response);
                return (true);
            }
            catch (Exception E)
            {
                this.Log(E);
                return (false);
            }
        }

        async Task<bool> IUser.SendNotificationAsync(int kind, string payload)
        {
            try
            {
                UserState state = await this.GetStateAsync<UserState>();
                if (!state.isCreated) return (false);
                var ev = GetEvent<IUserEvent>();
                ev.SendNotificationAsync(kind, payload);
                return (true);
            }
            catch (Exception E)
            {
                this.Log(E);
                return (false);
            }
        }
    }
}
