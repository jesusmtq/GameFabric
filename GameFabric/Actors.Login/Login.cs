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

namespace Actors.Login
{
    [StatePersistence(StatePersistence.Persisted)]
    internal class Login : Actor, ILogin
    {
        public Login(ActorService actorService, ActorId actorId)
            : base(actorService, actorId)
        {
        }
        protected override Task OnActivateAsync()
        {
            ActorEventSource.Current.ActorMessage(this, "Actor activated.");
            return this.TryCreateStateAsync<LoginState>(new LoginState());
        }
        async Task<bool> ILogin.ValidatePassword(string password)
        {
            try
            {
                LoginState state = await this.GetStateAsync<LoginState>();
                if (!password.ValidatePasswordHash(state.PasswordHash)) return (false);
                return (true);
            }
            catch (Exception E)
            {
                this.Log(E);
                return (false);
            }
        }
        async Task<bool> ILogin.Create(ActorId owningUserId, string password)
        {
            try
            {
                LoginState state = await this.GetStateAsync<LoginState>();
                state.UserActorId = owningUserId;
                state.PasswordHash = password.ToPasswordHash();
                await this.SetStateAsync(state);
                return (true);
            }
            catch (Exception E)
            {
                this.Log(E);
                return (false);
            }
        }
        async Task<bool> ILogin.SetPassword(string password)
        {
            try
            {
                LoginState state = await this.GetStateAsync<LoginState>();
                state.PasswordHash = password.ToPasswordHash();
                await this.SetStateAsync(state);
                return (true);
            }
            catch (Exception E)
            {
                this.Log(E);
                return (false);
            }
        }
        async Task<bool> ILogin.TryResetPassword(string oldPassword, string newPassword)
        {
            try
            {
                LoginState state = await this.GetStateAsync<LoginState>();
                if (!oldPassword.ValidatePasswordHash(state.PasswordHash)) return (false);
                state.PasswordHash = newPassword.ToPasswordHash();
                await this.SetStateAsync(state);
                return (true);
            }
            catch (Exception E)
            {
                this.Log(E);
                return (false);
            }
        }
        async Task<ActorId> ILogin.GetUserActorId()
        {
            try
            {
                LoginState state = await this.GetStateAsync<LoginState>();
                return (state.UserActorId);
            }
            catch (Exception E)
            {
                this.Log(E);
                throw (E);
            }
        }
        async Task ILogin.SetUserActorId(ActorId UserActorId)
        {
            try
            {
                LoginState state = await this.GetStateAsync<LoginState>();
                state.UserActorId = UserActorId;
                await this.SetStateAsync(state);
            }
            catch (Exception E)
            {
                this.Log(E);
                throw (E);
            }
        }
    }
}
