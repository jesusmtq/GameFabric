using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Client;
using Microsoft.ServiceFabric.Actors.Runtime;
using Microsoft.ServiceFabric.Data;
using GameFabric.Interfaces;

namespace GameFabric.Common
{
    public static class ActorExtensions
    {
        #region Proxies
        public static T Proxy<T>(this ActorId id, Uri serviceUri = null) where T : IActor
        {
            if (id == null)
            {
                throw new NullReferenceException($"Proxy<{typeof(T).FullName}> identity is null");
            }
            if(serviceUri == null) serviceUri = ServiceUriHelper.Resolve<T>();
            return ActorProxy.Create<T>(id, serviceUri);
        }

        public static async Task<bool> DeleteActor<T>(this ActorId id, Uri serviceUri = null) where T : IActor
        {
            CancellationToken token = new CancellationToken();
            if (serviceUri == null) serviceUri = ServiceUriHelper.Resolve<T>();
            var proxy = id.Proxy<T>(serviceUri);
            ActorReference actorReference = ActorReference.Get(proxy);
            var actorServiceProxy = ActorServiceProxy.Create(actorReference.ServiceUri, id);
            await actorServiceProxy.DeleteActorAsync(id, token);
            return await Task.FromResult(true);
        }
        #endregion

        #region States
        public static async Task<bool> TryCreateStateAsync<T>(this Actor actor, T defaultState, string stateName = "default")
        {
            return(await actor.StateManager.TryAddStateAsync(stateName, defaultState));
        }
        public static async Task<bool> CreateStateAsync<T>(this Actor actor, T defaultState, string stateName = "default")
        {
            bool result = await actor.StateManager.TryAddStateAsync(stateName, defaultState);
            if (!result) throw new Exception("Could not create state");
            return true;
        }
        public static async Task SetStateAsync<T>(this Actor actor, T state, string stateName = "default")
        {
            await actor.StateManager.SetStateAsync(stateName, state);
        }

        public static async Task SetStateAsync<T>(this Actor actor, T state,CancellationToken cancellationToken, string stateName = "default")
        {
            await actor.StateManager.SetStateAsync(stateName, state, cancellationToken);
        }
        public static async Task CreateOrSetStateAsync<T>(this Actor actor, T state, string stateName = "default")
        {
            if (await actor.ContainsStateAsync(stateName))
            {
                await actor.StateManager.SetStateAsync(stateName, state);
            }
            else
            {
                await actor.CreateStateAsync(state, stateName);
            }
        }
        public static async Task<T> GetStateAsync<T>(this Actor actor, string stateName = "default") where T : new()
        {
            ConditionalValue<T> result = await actor.StateManager.TryGetStateAsync<T>(stateName);
            if (result.HasValue) return result.Value;
            return new T();
        }

        public static async Task<T> GetStateAsync<T>(this Actor actor,CancellationToken cancelationToken, string stateName = "default") where T : new()
        {
            ConditionalValue<T> result = await actor.StateManager.TryGetStateAsync<T>(stateName, cancelationToken);
            if (result.HasValue) return result.Value;
            return new T();
        }

        public static async Task<bool> ContainsStateAsync(this Actor actor, string stateName = "default")
        {
            return await actor.StateManager.ContainsStateAsync(stateName);
        }

        public static async Task<bool> ContainsStateAsync(this Actor actor, CancellationToken cancelationToken, string stateName = "default")
        {
            return await actor.StateManager.ContainsStateAsync(stateName,cancelationToken);
        }
        public static async Task<bool> DeleteStateAsync(this Actor actor, string stateName = "default")
        {
            return await actor.StateManager.TryRemoveStateAsync(stateName);
        }
        #endregion
    }
}
