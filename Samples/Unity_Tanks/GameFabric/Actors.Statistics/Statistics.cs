using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Runtime;
using Microsoft.ServiceFabric.Actors.Client;
using GameFabric.Common;
using GameFabric.Common.Hashes;
using GameFabric.Interfaces;
using GameFabric.Models.SystemModels;
using GameFabric.Logging;

namespace Actors.Statistics
{

    [StatePersistence(StatePersistence.None)]
    internal class Statistics : Actor, IStatistics
    {

        public Statistics(ActorService actorService, ActorId actorId)
            : base(actorService, actorId)
        {
        }

        protected override Task OnActivateAsync()
        {
            ActorEventSource.Current.ActorMessage(this, "Actor activated.");
            return Task.FromResult(true);
        }


        async Task IStatistics.UpdateStats(string StatStorName, Dictionary<int, RequestStats> Data)
        {
            try
            {
                ActorId StoreId = StatStorName.ToActorId();
                Dictionary<int, RequestStats> Stored = await StoreId.LoadGeneric<Dictionary<int, RequestStats>>();
                if (Stored == null) Stored = new Dictionary<int, RequestStats>();
                if (Data != null)
                {
                    foreach (int k in Data.Keys)
                    {
                        if (!Stored.ContainsKey(k)) Stored.Add(k, new RequestStats(k));
                        Stored[k].CompressedSize += Data[k].CompressedSize;
                        Stored[k].NumCalls += Data[k].NumCalls;
                        Stored[k].TotalTime += Data[k].TotalTime;
                        Stored[k].TotalSize += Data[k].TotalSize;
                    }
                    await Stored.SaveGeneric(StoreId);
                }
            }
            catch (Exception E)
            {
                this.Log(E);
                throw (E);
            }
        }

        async Task<List<RequestStats>> IStatistics.GetStatistics(string StatStorName)
        {
            try
            {
                ActorId StoreId = StatStorName.ToActorId();
                Dictionary<int, RequestStats> Stored = await StoreId.LoadGeneric<Dictionary<int, RequestStats>>();
                List<RequestStats> toReturn = new List<RequestStats>();
                foreach (int k in Stored.Keys)
                {
                    toReturn.Add(Stored[k]);
                }
                return (toReturn);
            }
            catch (Exception E)
            {
                this.Log(E);
                throw (E);
            }
        }

        async Task<string> IStatistics.GetStatisticsJSon(string StatStorName)
        {
            try
            {
                ActorId StoreId = StatStorName.ToActorId();
                Dictionary<int, RequestStats> Stored = await StoreId.LoadGeneric<Dictionary<int, RequestStats>>();
                List<RequestStats> toReturn = new List<RequestStats>();
                foreach (int k in Stored.Keys)
                {
                    toReturn.Add(Stored[k]);
                }
                return (toReturn.Serialize());
            }
            catch (Exception E)
            {
                this.Log(E);
                throw (E);
            }
        }
    }
}
