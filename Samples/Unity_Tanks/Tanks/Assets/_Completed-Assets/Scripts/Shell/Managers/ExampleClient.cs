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
    public enum CallFailEnum:int { CallInvalid=0, CallFailed=1}
    public class ExampleSignalRManager
    {
        public SignalRHubClient m_client;           //SignalRClient

        #region private vars
        private string _SessionKey;                 //SessionKey
        private string _UserName;                   //UserName
        private string _Password;                   //Password
        private int _recievedResonses = 0;
        public Guid _UserId = Guid.Empty;           //User identity
        #endregion

        #region public properties
        public string ServerUrl { get; set; }
        public string SignaRHubName { get; set; }
        public int RecievedResponses { get { return (_recievedResonses); } }
        public Guid UserId { get { return (_UserId); } }
        #endregion

        #region Actions
        public Action<Guid> OnUserLoggedIn;
        public Action<Exception> OnCallException;
        public Action<int, CallFailEnum> OnCallFail;
        public Action<Guid, string> OnMessage;
        #endregion
        //Default creator
        public ExampleSignalRManager()
        {
            ServerUrl = "clustername.location.cloudapp.azure.com/signalr/";
            SignaRHubName = "InterfaceHub";
        }
        public bool InitializeAndConnect(string UserName, string Password)
        {
            try
            {
                _UserName = UserName;
                _Password = Password;
                m_client = new SignalRHubClient(ServerUrl, SignaRHubName, false);
                //Map hub Actions
                m_client.On<string>("Exec", s =>
                {
                    ClientGateResponse resp = s.Deserialize<ClientGateResponse>();
                    this.ProcessResponse(resp);
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
            if (response.ResultCode == (int)HttpStatusCode.OK)
            {
                switch ((RequestProcessorEnum)response.Kind)
                {
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
                                m_client.Invoke("Exec", lr.CreateRequest(RequestProcessorEnum.LoginUser, _UserId).Serialize());
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
                        else
                        {
                            if (OnCallFail != null) OnCallFail(response.Kind,CallFailEnum.CallFailed);
                        }
                        break;
                    case RequestProcessorEnum.LoginUser:
                        {
                            LoginUserResponse lur = response.JsonPayload.Deserialize<LoginUserResponse>();
                            if (lur.Status == HttpStatusCode.OK)
                            {
                                _UserId = lur.UserId;
                                if (OnUserLoggedIn!=null) OnUserLoggedIn(_UserId);
                            }
                            else
                            {
                                if (OnCallFail != null) OnCallFail(response.Kind, CallFailEnum.CallFailed);
                            }
                        }
                        break;
                    case RequestProcessorEnum.SendMessage:
                        {
                            SendMessageResponse messageResponse = response.JsonPayload.Deserialize<SendMessageResponse>();
                            if (OnMessage != null) OnMessage(messageResponse.FromUserId, messageResponse.Message);
                        }
                        break;
                    default:
                        break;
                }
            }
            UnityEngine.Debug.Log(response.Kind.ToString() + " time:" + response.TimeTaken.ToString() + " Json:" + response.JsonPayload);
        }
        public void StartLogin()
        {
            UserExistsRequest uer = new UserExistsRequest();
            uer.UserName = _UserName;
            m_client.Invoke("Exec", uer.CreateRequest(RequestProcessorEnum.UserExists, _UserId).Serialize());
        }

        public void SendMessageTo(Guid toUserId,string message)
        {
            SendMessageRequest messageRequest = new SendMessageRequest();
            messageRequest.FromUserId = this.UserId;
            messageRequest.ToUserId = toUserId;
            messageRequest.Message = message;
            m_client.Invoke("Que", messageRequest.CreateRequest(RequestProcessorEnum.SendMessage, _UserId).Serialize());
        }
    }
}

