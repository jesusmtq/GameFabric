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
using GameFabric.Processors;
using GameFabric.Shared.Responses;
using GameFabric.Shared;
using GameFabric.Processors.Interfaces;
using GameFabric.Logging;
using System.Net;

namespace Actors.TankGameSession
{
    [StatePersistence(StatePersistence.Persisted)]
    internal class TankGameSession : Actor, ITankGameSession
    {
        public TankGameSession(ActorService actorService, ActorId actorId)
            : base(actorService, actorId)
        {
        }
        protected override Task OnActivateAsync()
        {
            ActorEventSource.Current.ActorMessage(this, "Actor activated.");
            return this.TryCreateStateAsync<GameSessionState>(new GameSessionState());
        }
        async Task<int> ITankGameSession.JoinAsync(ActorId UserId)
        {
            try
            {
                GameSessionState state = await this.GetStateAsync<GameSessionState>();
                if (state.Players != null && state.Players.Count < 2)
                {
                    int seq = state.Players.Count + 1;
                    state.Players.Add(new TankSessionMemberItem(UserId, seq));
                    await this.SetStateAsync(state);
                    return (seq);
                }
                else
                {
                    return (0);
                }
            }
            catch (Exception E)
            {
                this.Log(E);
                return (0);
            }
        }
        async Task ITankGameSession.NotifyPosition(ActorId TankId, float x, float y, float z, float r)
        {
            try
            {
                GameSessionState state = await this.GetStateAsync<GameSessionState>();
                List<Task> notes = new List<Task>();

                TankPosistionResponse resp = new TankPosistionResponse(TankId.GetGuidId(), x, y, z, r);
                string serialized = resp.Serialize();
                List<ActorId> TargetUids = state.Players.Select(p => p.UserId).Distinct().ToList();
                foreach (TankSessionMemberItem i in state.Players)
                {
                    if (i.TankId != TankId)
                    {
                        GateResponse gr = new GateResponse((int)RequestProcessorEnum.TankPosition, (int)HttpStatusCode.OK, i.UserId.GetGuidId(), serialized);
                        await i.UserId.Proxy<IUser>().SendGateResponseAsync(gr);
                    }
                }
            }
            catch (Exception E)
            {
                this.Log(E);
            }
        }
        async Task ITankGameSession.NotifyFireShell(ActorId TankId, string SerializedData)
        {
            try
            {
                GameSessionState state = await this.GetStateAsync<GameSessionState>();
                List<ActorId> TargetUids = state.Players.Select(p => p.UserId).Distinct().ToList();
                foreach (TankSessionMemberItem i in state.Players)
                {
                    GateResponse gr = new GateResponse((int)RequestProcessorEnum.FireShell, (int)HttpStatusCode.OK, i.UserId.GetGuidId(), SerializedData);
                    await i.UserId.Proxy<IUser>().SendGateResponseAsync(gr);
                }
            }
            catch (Exception E)
            {
                this.Log(E);
            }
        }
        async Task<int> ITankGameSession.GetPlayerCountAsync(CancellationToken cancellationToken)
        {
            try
            {
                GameSessionState state = await this.GetStateAsync<GameSessionState>(cancellationToken);
                if (state.Players != null) return (state.Players.Count);
                return (0);
            }
            catch (Exception E)
            {
                this.Log(E);
                return (0);
            }
        }
        async Task<bool> ITankGameSession.CanStart(CancellationToken cancellationToken)
        {
            try
            {
                GameSessionState state = await this.GetStateAsync<GameSessionState>(cancellationToken);
                if (state.Players != null && state.Players.Count == 2) return (true);
                return (false);
            }
            catch (Exception E)
            {
                this.Log(E);
                return (false);
            }
        }
        async Task<bool> ITankGameSession.StartSession(CancellationToken cancellationToken)
        {
            try
            {
                GameSessionState state = await this.GetStateAsync<GameSessionState>(cancellationToken);
                List<Task> tasks = new List<Task>();
                foreach (TankSessionMemberItem i in state.Players)
                {
                    tasks.Add(i.TankId.Proxy<ITank>().ResetAsync());
                }
                await Task.WhenAll(tasks);
                state.PlayerOneScore = 0;
                state.PlayerTwoScore = 0;
                await this.SetStateAsync(state, cancellationToken);
                return (true);
            }
            catch (Exception E)
            {
                this.Log(E);
                return (false);
            }
        }
        async Task<List<TankSessionMemberItem>> ITankGameSession.GetPlayersAsync()
        {
            try
            {
                GameSessionState state = await this.GetStateAsync<GameSessionState>();
                return (state.Players);
            }
            catch (Exception E)
            {
                this.Log(E);
                return (new List<TankSessionMemberItem>());
            }
        }
        async Task<bool> ITankGameSession.StartRound(ActorId aUserId)
        {
            try
            {
                GameSessionState state = await this.GetStateAsync<GameSessionState>();
                List<Task> tasks = new List<Task>();
                int currentuserround = 0;
                foreach (TankSessionMemberItem i in state.Players)
                {
                    tasks.Add(i.TankId.Proxy<ITank>().ResetAsync());
                    if (i.UserId == aUserId)
                    {
                        i.CurrentRound++;
                        currentuserround = i.CurrentRound;
                    }
                }
                await Task.WhenAll(tasks);
                await this.SetStateAsync(state);


                //And notify
                StartRoundResponse srr = new StartRoundResponse();
                srr.RoundNum = currentuserround;
                srr.Status = HttpStatusCode.OK;
                srr.UserId = aUserId.GetGuidId();
                foreach (TankSessionMemberItem i in state.Players)
                {
                    GateResponse gr = new GateResponse((int)RequestProcessorEnum.StartRound, (int)HttpStatusCode.OK, i.UserId.GetGuidId(), srr.Serialize());
                    await i.UserId.Proxy<IUser>().SendGateResponseAsync(gr);
                }

                bool OnSameRound = true;
                for (int i = 0; i < state.Players.Count - 1; i++)
                {
                    if (state.Players[i].CurrentRound != state.Players[i + 1].CurrentRound) OnSameRound = false;
                }
                if (OnSameRound)
                {
                    BeginRounResponse brr = new BeginRounResponse(this.Id.GetGuidId(), currentuserround);
                    brr.Status = HttpStatusCode.OK;
                    foreach (TankSessionMemberItem i in state.Players)
                    {
                        GateResponse gr = new GateResponse((int)RequestProcessorEnum.BeginRound, (int)HttpStatusCode.OK, i.UserId.GetGuidId(), brr.Serialize());
                        await i.UserId.Proxy<IUser>().SendGateResponseAsync(gr);
                    }
                }

                return (true);
            }
            catch (Exception E)
            {
                this.Log(E);
                return (false);
            }
        }

        async Task<bool> ITankGameSession.CanStartRound(int roundNo)
        {
            try
            {
                GameSessionState state = await this.GetStateAsync<GameSessionState>();
                bool areonround = true;
                foreach (TankSessionMemberItem i in state.Players)
                {
                    if (i.CurrentRound != roundNo) areonround = false;
                }
                return (areonround);
            }
            catch (Exception E)
            {
                this.Log(E);
                return (false);
                throw;
            }
        }

        async Task<bool> ITankGameSession.FinishRound()
        {
            try
            {
                GameSessionState state = await this.GetStateAsync<GameSessionState>();
                //state.TotalRounds++;
                await this.SetStateAsync(state);
                return (true);
            }
            catch (Exception E)
            {
                this.Log(E);
                return (false);
            }
        }
        async Task<bool> ITankGameSession.TakeDamage(ActorId TankID, float amount)
        {
            try
            {
                bool isDead = false;
                float newHealth = 0.0f;
                GameSessionState state = await this.GetStateAsync<GameSessionState>();
                foreach (TankSessionMemberItem i in state.Players)
                {
                    if (i.TankId == TankID)
                    {
                        newHealth = await (i.TankId.Proxy<ITank>().TakeDamageAsync(amount));
                        if (newHealth <= 0) isDead = true;
                    }
                }
                await this.SetStateAsync(state);
                //Notify clients
                TakeDamageResponse tdr = new TakeDamageResponse();
                tdr.Status = System.Net.HttpStatusCode.OK;
                tdr.TankGameSessionId = this.Id.GetGuidId();
                tdr.TankId = TankID.GetGuidId();
                tdr.Health = newHealth;
                tdr.isDead = isDead;

                List<Task> tasks = new List<Task>();
                foreach (TankSessionMemberItem i in state.Players)
                {
                    GateResponse gr = new GateResponse((int)RequestProcessorEnum.TakeDamage, (int)HttpStatusCode.OK, i.UserId.GetGuidId(), tdr.Serialize());
                    tasks.Add(i.UserId.Proxy<IUser>().SendGateResponseAsync(gr));
                }
                await Task.WhenAll(tasks);
                //End notify clients
                return (isDead);
            }
            catch (Exception E)
            {
                this.Log(E);
                return (false);
            }
        }

    }
}
