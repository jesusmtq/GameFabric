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
using GameFabric.Models;
using GameFabric.Models.SystemModels;
using GameFabric.Logging;

namespace Actors.TankGameSessionList
{
    [StatePersistence(StatePersistence.Persisted)]
    internal class TankGameSessionList : Actor, ITankGameSessionList
    {

        public TankGameSessionList(ActorService actorService, ActorId actorId)
            : base(actorService, actorId)
        {
        }


        protected override Task OnActivateAsync()
        {

            ActorEventSource.Current.ActorMessage(this, "Actor activated.");
            return this.TryCreateStateAsync<List<GameSessionListItem>>(new List<GameSessionListItem>());
        }
        async Task<bool> ITankGameSessionList.AddGameSessionAsync(ActorId GameSessionId)
        {
            try
            {
                List<GameSessionListItem> state = await this.GetStateAsync<List<GameSessionListItem>>();
                state.Add(new GameSessionListItem(GameSessionId));
                await this.SetStateAsync(state);
                return (true);
            }
            catch (Exception E)
            {
                this.Log(E);
                return (false);
            }
        }
        async Task<GameSessionListItem> ITankGameSessionList.GetNextSessionAsync()
        {
            try
            {
                List<GameSessionListItem> state = await this.GetStateAsync<List<GameSessionListItem>>();
                if (state.Count == 0) return (null);
                //filter any old sessions
                List<GameSessionListItem> filtered = new List<GameSessionListItem>();
                foreach (GameSessionListItem i in state)
                {
                    if (i.ValidUntil > DateTime.UtcNow) filtered.Add(i);
                }

                GameSessionListItem itm = filtered.OrderBy(p => p.Created).FirstOrDefault();
                await this.SetStateAsync(filtered);
                return (itm);
            }
            catch (Exception E)
            {
                this.Log(E);
                return (null);
            }
        }
        async Task<bool> ITankGameSessionList.HasSessionsAsync()
        {
            try
            {
                List<GameSessionListItem> state = await this.GetStateAsync<List<GameSessionListItem>>();
                if (state.Count == 0) return (false);
                //filter any old sessions
                List<GameSessionListItem> filtered = new List<GameSessionListItem>();
                foreach (GameSessionListItem i in state)
                {
                    if (i.ValidUntil > DateTime.UtcNow) filtered.Add(i);
                }
                await this.SetStateAsync(filtered);
                if (filtered.Count > 0) return (true);
                return (false);
            }
            catch (Exception E)
            {
                this.Log(E);
                return (false);
            }
        }

        async Task<bool> ITankGameSessionList.RemoveGameSessionAsync(ActorId GameSessionId)
        {
            try
            {
                List<GameSessionListItem> state = await this.GetStateAsync<List<GameSessionListItem>>();
                List<GameSessionListItem> filtered = new List<GameSessionListItem>();
                foreach (GameSessionListItem i in state)
                {
                    if (i.GameSessionId != GameSessionId) filtered.Add(i);
                }
                await this.SetStateAsync(filtered);
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
