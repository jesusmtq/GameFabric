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

namespace Actors.GenericStorage
{
    [StatePersistence(StatePersistence.Persisted)]
    internal class GenericStorage : Actor, IGenericStorage
    {
        public GenericStorage(ActorService actorService, ActorId actorId)
             : base(actorService, actorId)
        {
        }

        protected override Task OnActivateAsync()
        {
            ActorEventSource.Current.ActorMessage(this, "Actor activated.");
            return this.TryCreateStateAsync<GenericStorageState>(new GenericStorageState());
        }

        async Task<bool> IGenericStorage.ContainsDataAsync()
        {
            try
            {
                GenericStorageState state = await this.GetStateAsync<GenericStorageState>();
                return (state.ContainsData);
            }
            catch (Exception E)
            {
                this.Log(E);
                return (false);
            }
        }
        async Task<string> IGenericStorage.GetDataAsync()
        {
            try
            {
                GenericStorageState state = await this.GetStateAsync<GenericStorageState>();
                return (state.Data);
            }
            catch (Exception E)
            {
                this.Log(E);
                throw (E);
            }
        }
        async Task<bool> IGenericStorage.SetDataAsync(string Data)
        {
            try
            {
                GenericStorageState state = await this.GetStateAsync<GenericStorageState>();
                state.Data = Data;
                state.ContainsData = true;
                state.Changed = DateTime.UtcNow;
                await this.SetStateAsync(state);
                return (true);
            }
            catch (Exception E)
            {
                this.Log(E);
                return (false);
            }
        }
        async Task<bool> IGenericStorage.DeleteDataAsync()
        {
            try
            {
                GenericStorageState state = await this.GetStateAsync<GenericStorageState>();
                state.Data = string.Empty;
                state.ContainsData = false;
                state.Changed = DateTime.UtcNow;
                await this.SetStateAsync(state);
                return (true);
            }
            catch (Exception E)
            {
                this.Log(E);
                return (false);
            }
        }
        async Task<DateTime> IGenericStorage.LastModifiedAsync()
        {
            try
            {
                GenericStorageState state = await this.GetStateAsync<GenericStorageState>();
                return (state.Changed);
            }
            catch (Exception E)
            {
                this.Log(E);
                throw (E);
            }
        }
        async Task<DateTime> IGenericStorage.CreatedAsync()
        {
            try
            {
                GenericStorageState state = await this.GetStateAsync<GenericStorageState>();
                return (state.Created);
            }
            catch (Exception E)
            {
                this.Log(E);
                throw (E);
            }
        }

    }
}
