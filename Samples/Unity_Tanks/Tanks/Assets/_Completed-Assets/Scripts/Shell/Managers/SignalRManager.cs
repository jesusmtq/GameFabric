using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Newtonsoft.Json;
using Unity.MiniSignalR;
using GameFabric.Shared;
using GameFabric.Shared.Requests;
using GameFabric.Shared.Responses;
using GameFabric.Processors.Interfaces;
using GameFabric.Processors;
using System.Net;

namespace Complete
{
    public class SignalRManager
    {
        public SignalRHubClient m_client;                       //SignalRClient

        #region private vars
        private ClientGateResponse _LastResponse;               //Last response
        public Guid _UserId=Guid.Empty;                         //User identity
        private string _SessionKey;                             //SessionKey
        private string _UserName;                               //UserName
        private string _Password;                               //Password
        private bool _isMapped = false;                         //Track if mapped on server or not
        private Guid _CurrentGameSessionId = Guid.Empty;        //Current tank game session id
        private int _recievedResonses = 0;
        #endregion

        #region public properties
        public string ServerUrl { get; set; }
        public string SignaRHubName { get; set; }
        public int RecievedResponses { get { return (_recievedResonses); } }
        #endregion

        #region Actions
        public Action<Guid,Vector3,float> OnActionTrack;
        public Action<Vector3, Quaternion, Vector3> OnActionFire;
        public Action<List<GameSessionPlayerItem>> OnStartSession;
        public Action<Guid,float> OnActionSetDamage;
        public Action<Guid,int> OnStartRound;
        public Action<int> OnRoundBegins;
        #endregion

        //Default creator
        public SignalRManager()
        {
            ServerUrl = "gamefabric.westeurope.cloudapp.azure.com/signalr/";// "localhost/signalr/";
            SignaRHubName = "InterfaceHub";
        }
        public bool InitializeAndConnect(string UserName,string Password)
        {
            try
            {
                _UserName = UserName;
                _Password = Password;
                
                m_client = new SignalRHubClient(ServerUrl, SignaRHubName, false);
                //Map hub events
                m_client.On<string>("Exec", s =>
                {
                    ClientGateResponse resp = s.Deserialize<ClientGateResponse>();
                    _LastResponse = resp;
                    this.ProcessResponse(resp);
                });
                m_client.On<string>("Map", s =>
                {
                    _isMapped = true;
                    JoinOrCreateSession();
                });
                //Open connection
                m_client.Open();
                return (true);
            }
            catch (Exception E)
            {
                throw (E);
            }
        }
        public void ProcessResponse(ClientGateResponse response)
        {
            _recievedResonses++;
            if (response.ResultCode==(int)HttpStatusCode.OK)
            {
                switch ((RequestProcessorEnum)response.Kind)
                {
                    
                    case RequestProcessorEnum.Session:
                        {
                            CreateSessionResponse csresp= response.JsonPayload.Deserialize<CreateSessionResponse>();
                            if (csresp.Status==HttpStatusCode.OK)
                            {
                                _SessionKey = csresp.SessionKey;
                                m_client.Invoke("Map", _UserId.ToString());
                            }
                        }
                        break;
                    case RequestProcessorEnum.UserExists:
                        {
                            UserExistsResponse r = response.JsonPayload.Deserialize<UserExistsResponse>();
                            if (!r.Exists)
                            {
                                //Create user
                                CreateUserRequest cr = new CreateUserRequest();
                                cr.UserName = _UserName;
                                cr.Password = _Password;
                                m_client.Invoke("Exec", cr.CreateRequest(RequestProcessorEnum.CreateUser, _UserId).Serialize());
                            }
                            else
                            {
                                LoginUserRequest lr = new LoginUserRequest();
                                lr.UserName = _UserName;
                                lr.Password = _Password;
                                m_client.Invoke("Exec", lr.CreateRequest( RequestProcessorEnum.LoginUser, _UserId).Serialize());
                            }
                        }
                        break;
                    case RequestProcessorEnum.CreateUser:
                        CreateUserResponse cresp = response.JsonPayload.Deserialize<CreateUserResponse>();
                        if (cresp.Sucessful)
                        {
                            _UserId = cresp.UserId;
                            CreateSessionRequest csesr = new CreateSessionRequest();
                            csesr.UserId = _UserId;
                            csesr.SessionKey = string.Empty;
                            m_client.Invoke("Exec", csesr.CreateRequest(RequestProcessorEnum.Session, _UserId).Serialize());

                        }
                        break;
                    case RequestProcessorEnum.LoginUser:
                        {
                            LoginUserResponse lur = response.JsonPayload.Deserialize<LoginUserResponse>();
                            if (lur.Status==HttpStatusCode.OK)
                            {
                                _UserId = lur.UserId;
                                CreateSessionRequest csesr = new CreateSessionRequest();
                                csesr.UserId = _UserId;
                                csesr.SessionKey = string.Empty;
                                m_client.Invoke("Exec", csesr.CreateRequest(RequestProcessorEnum.Session, _UserId).Serialize());
                            }
                        }
                        break;
                    case RequestProcessorEnum.TankPosition:
                        {
                            TankPosistionResponse tr= response.JsonPayload.Deserialize<TankPosistionResponse>();
                            if (OnActionTrack!=null) OnActionTrack(tr.TankId,new Vector3(tr.x, tr.y, tr.z),tr.r);
                        }
                        break;
                    case RequestProcessorEnum.FireShell:
                        {
                            FireShellResponse fr = response.JsonPayload.Deserialize<FireShellResponse>();
                            if (OnActionFire!=null)
                            {
                                OnActionFire(new Vector3(fr.pos.x, fr.pos.y, fr.pos.z),
                                    new Quaternion(fr.rot.x, fr.rot.y, fr.rot.z, fr.rot.w),
                                    new Vector3(fr.vel.x, fr.vel.y, fr.vel.z));
                            }
                        }
                        break;
                    case RequestProcessorEnum._System_MapConnection:
                        {
                            //Map
                            _isMapped = true;
                        }
                        break;
                    case RequestProcessorEnum.TakeDamage:
                        {
                            TakeDamageResponse tdr = response.JsonPayload.Deserialize<TakeDamageResponse>();
                            if (OnActionSetDamage != null) OnActionSetDamage(tdr.TankId, tdr.Health);
                        }
                        break;
                    case RequestProcessorEnum.JoinOrCreateGame:
                        {
                            //Join or create game session
                            JoinOrCreateGameSessionResponse jcr = response.JsonPayload.Deserialize<JoinOrCreateGameSessionResponse>();
                            //Test if to run or wait
                            _CurrentGameSessionId = jcr.GameSessionId;
                            if (jcr.start)
                            {
                                int cnt = jcr.SessionPlayers.Count;
                                if (OnStartSession!=null) OnStartSession(jcr.SessionPlayers);
                            }
                            else
                            {
                                //used for single instance debug
                                //Create second request and link to same session enable single instance tests
                                //JoinOrCreateGameSessionRequest jcrr = new JoinOrCreateGameSessionRequest();
                                //jcrr.UserId = _UserId;
                                //m_client.Invoke("Que", jcrr.CreateRequest( RequestProcessorEnum.JoinOrCreateGame, _UserId).Serialize());
                            }
                        }
                        break;
                    case RequestProcessorEnum.StartRound:
                        {
                            StartRoundResponse srr = response.JsonPayload.Deserialize<StartRoundResponse>();
                            if (OnStartRound!=null) OnStartRound(srr.TankId, srr.RoundNum);
                        }
                        break;
                    case RequestProcessorEnum.BeginRound:
                        {
                            BeginRounResponse brr = response.JsonPayload.Deserialize<BeginRounResponse>();
                            if (OnRoundBegins != null) OnRoundBegins(brr.RoundNum);
                        }
                        break;
                    case RequestProcessorEnum.GetCanStartRound:
                        {
                            CanStartRoundResponse crr = response.JsonPayload.Deserialize<CanStartRoundResponse>();
                            if (OnRoundBegins != null) OnRoundBegins(crr.RoundNum);
                        }
                        break;
                    default:
                        break;
                }
            }
           if (response.Kind!=(int)RequestProcessorEnum.TankPosition) UnityEngine.Debug.Log(response.Kind.ToString()+" time:"+response.TimeTaken.ToString()+" total calls:"+_recievedResonses.ToString()+" Json:"+response.JsonPayload);
            //UnityEngine.Debug.Log(response.Kind.ToString() + " time:" + response.TimeTaken.ToString() + " Json:" + response.JsonPayload);
        }

        public void StartRound()
        {
            StartRoundRequest srr = new StartRoundRequest();
            srr.TankGameSessionId = _CurrentGameSessionId;
            srr.UserId = _UserId;
            m_client.Invoke("Que", srr.CreateRequest(RequestProcessorEnum.StartRound, _UserId).Serialize());
        }

        public void CanStartRound(int roundnum)
        {
            
            CanStartRoundRequest csr = new CanStartRoundRequest(this._CurrentGameSessionId,roundnum);
            m_client.Invoke("Exec", csr.CreateRequest(RequestProcessorEnum.GetCanStartRound, _UserId).Serialize());
        }

        public void JoinOrCreateSession()
        {
            try
            {
                JoinOrCreateGameSessionRequest jcr = new JoinOrCreateGameSessionRequest();
                jcr.UserId = _UserId;
                //m_client.Invoke("Exec", jcr.CreateRequest(RequestProcessorEnum.JoinOrCreateGame, _UserId).Serialize());
                m_client.Invoke("Que", jcr.CreateRequest(RequestProcessorEnum.JoinOrCreateGame, _UserId).Serialize());
            }
            catch (Exception E)
            {

            }
        }

        public void TrackPosition(Guid TankId,float x,float y,float z,float r)
        {
            try
            {
                TankPositionQueRequest tqr = new TankPositionQueRequest(_CurrentGameSessionId, TankId, x, y, z, r);
                m_client.Invoke("Que", tqr.CreateRequest( RequestProcessorEnum.TankPosition,_UserId).Serialize());
            }
            catch (Exception E)
            {
                UnityEngine.Debug.Log(E.Message);
            }
        }

        public void TakeDamage(Guid TankId,float Amount)
        {
            try
            {
                TakeDamageRequest tdr = new TakeDamageRequest(); 
                tdr.Amount = Amount;
                tdr.TankGameSessionId = _CurrentGameSessionId;
                tdr.TankId = TankId;
                m_client.Invoke("Que", tdr.CreateRequest( RequestProcessorEnum.TakeDamage, _UserId).Serialize());
            }
            catch (Exception E)
            {
                UnityEngine.Debug.Log(E.Message);
            }
        }

        public void StartConnect()
        {
            UserExistsRequest uer = new UserExistsRequest();
            uer.UserName = _UserName;
            m_client.Invoke("Exec", uer.CreateRequest( RequestProcessorEnum.UserExists,_UserId).Serialize());
        }

        public void FireRequest(Guid TankId,Vector3 pos,Quaternion rot,Vector3 vel)
        {
            FireShellRequest rq = new GameFabric.Shared.Requests.FireShellRequest();
            rq.pos = new clientVector3(pos.x, pos.y,pos.z);
            rq.rot = new clientQuaternion(rot.x, rot.y, rot.z, rot.w);
            rq.vel = new clientVector3(vel.x, vel.y, vel.z);
            rq.TankId = TankId;
            rq.TankGameSessionId = this._CurrentGameSessionId;
            m_client.Invoke("Que", rq.CreateRequest(RequestProcessorEnum.FireShell,_UserId).Serialize());
        }
    }

    public static class RequestExtensions
    {
        public static ClientGateRequest CreateRequest(this IRequest request,RequestProcessorEnum kind,Guid UserId)
        {
            ClientGateRequest r = new ClientGateRequest();
            r.Kind =(int) kind;
            r.JsonPayload = JsonConvert.SerializeObject(request);
            return (r);
        }

        public static string Serialize(this object toSerialize)
        {
            return (JsonConvert.SerializeObject(toSerialize));
        }

        public static T Deserialize<T>(this string Json)
        {
            return (JsonConvert.DeserializeObject<T>(Json));
        }
    }
}
