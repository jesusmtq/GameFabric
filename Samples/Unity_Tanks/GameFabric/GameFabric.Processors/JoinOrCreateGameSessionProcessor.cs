using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameFabric.Common;
using GameFabric.Interfaces;
using GameFabric.Processors;
using GameFabric.Common.Attributes;
using GameFabric.Processors.Interfaces;
using GameFabric.Shared.Responses;
using GameFabric.Shared.Requests;
using Microsoft.ServiceFabric.Actors;
using GameFabric.Common.Hashes;
using System.Net;
using GameFabric.Models;
using GameFabric.Models.SystemModels;
using System.Threading;
using GameFabric.Logging;

namespace GameFabric.Processors
{
    [ReqestProcessor("JoinOrCreateGameSession", (int)RequestProcessorEnum.JoinOrCreateGame, typeof(JoinOrCreateGameSessionRequest), typeof(JoinOrCreateGameSessionResponse))]
    public class JoinOrCreateGameSessionProcessor: IRequestProcessor
    {
        public int ProcessorId
        {
            get
            {
                return ((int)RequestProcessorEnum.JoinOrCreateGame);
            }
        }
        public bool Authenticated { get { return (false); } }
        public async Task<IResponse> Process(IRequest request)
        {
            try
            {
                
                JoinOrCreateGameSessionRequest rq = request as JoinOrCreateGameSessionRequest;
                JoinOrCreateGameSessionResponse response = new JoinOrCreateGameSessionResponse();
                List<ActorId> NotifyList = new List<ActorId>();
                //test if there is a session
                var listproxy = new ActorId(1).Proxy<ITankGameSessionList>();
                if (await listproxy.HasSessionsAsync())
                {
                    GameSessionListItem itm = await listproxy.GetNextSessionAsync();
                    if (itm!=null)
                    {
                        response.GameSessionId = itm.GameSessionId.GetGuidId();
                        response.Status = HttpStatusCode.OK;
                        response.waitForPlayers = false;
                        response.start = true;
                        await listproxy.RemoveGameSessionAsync(itm.GameSessionId);
                        var sessionproxy = itm.GameSessionId.Proxy<ITankGameSession>();
                        int sequenceno=await sessionproxy.JoinAsync(new ActorId(rq.UserId));
                        //Get players 
                        List<TankSessionMemberItem> members = await sessionproxy.GetPlayersAsync();
                        foreach (TankSessionMemberItem i in members) { response.SessionPlayers.Add(new GameSessionPlayerItem(i.UserId.GetGuidId(), i.TankId.GetGuidId(), i.Sequence)); NotifyList.Add(i.UserId); }
                    }
                    else
                    {
                        response.Status = HttpStatusCode.InternalServerError;
                        ErrorResponse errorresponse = new ErrorResponse("Failed to create gamesession");
                        return (errorresponse);
                    }
                }
                else
                {
                    Guid newSessionid = Guid.NewGuid();
                    ActorId sessionActorId = new ActorId(newSessionid);
                    if (await listproxy.AddGameSessionAsync(sessionActorId))
                    {
                        //Set Session Params
                        var sessionproxy = sessionActorId.Proxy<ITankGameSession>();
                        int sequenceno = await sessionActorId.Proxy<ITankGameSession>().JoinAsync(new ActorId(rq.UserId));
                        response.GameSessionId = newSessionid;
                        response.Status = HttpStatusCode.OK;
                        response.waitForPlayers = true;
                        response.start = false;
                        //Get players 
                        List<TankSessionMemberItem> members = await sessionproxy.GetPlayersAsync();
                        foreach (TankSessionMemberItem i in members) { response.SessionPlayers.Add(new GameSessionPlayerItem(i.UserId.GetGuidId(), i.TankId.GetGuidId(), i.Sequence)); NotifyList.Add(i.UserId); }
                    }
                    else
                    {
                        response.Status = HttpStatusCode.InternalServerError;
                        ErrorResponse errorresponse = new ErrorResponse("Failed to create gamesession");
                        return (errorresponse);
                    }
                }
                response.Status = HttpStatusCode.OK;
                //Notidy participants
                if (NotifyList.Count == 0) NotifyList.Add(new ActorId(rq.UserId));
                foreach (ActorId uid in NotifyList)
                {
                    GateResponse gr = new GateResponse(this.ProcessorId, (int)System.Net.HttpStatusCode.OK, uid.GetGuidId(), response.Serialize());
                    await uid.Proxy<IUser>().SendGateResponseAsync(gr);
                }
                return (await Task.FromResult(response));
            }
            catch (Exception E)
            {
                E.Log();
                ErrorResponse errorresponse = new ErrorResponse(E.Message);
                return (errorresponse);
            }
        }
    }
}
