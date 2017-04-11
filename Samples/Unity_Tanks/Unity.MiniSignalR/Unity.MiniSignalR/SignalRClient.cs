using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WebSocketSharp;
using System.Net;
using System.IO;
using Newtonsoft.Json;

namespace Unity.MiniSignalR
{
    /// <summary>
    /// Simple SignalR client for Unity3d
    /// </summary>
    public class SignalRHubClient
    {

        #region private vars
        private string _Host = string.Empty;
        private bool _UseHttps = false;
        private string _HubName = string.Empty;
        private string httphost = string.Empty;
        private string wshost = string.Empty;

        private Dictionary<string, UnTypedActionContainer> _actionMap;
        private WebSocket _ws;
        private string _connectionToken;
        private NegotiateResponse _NegotiatedResponse;

        private int _RecievedMessages = 0;
        private int _RecievedHubMessages = 0;
        private int _SentMessages = 0;
        private int _RecievedHubErrors = 0;
        #endregion

        #region public properties

        public string Host { get { return (_Host); } }
        public bool UseHttps { get { return (_UseHttps); } }
        public string ConnectionToken { get { return (_connectionToken); } }
        public NegotiateResponse NegotiatedResponse { get { return (_NegotiatedResponse); } }
        public bool Connected { get { return (_ws.ReadyState == WebSocketState.Open); } }
        public int RecievedMessages { get { return (_RecievedMessages); } }
        public int RecievedHubMessages { get { return (_RecievedHubMessages); } }
        public int SentMessages { get { return (_SentMessages); } }
        public int RecievedHubErrors { get { return (_RecievedHubErrors); } }
        #endregion

        #region Actions
        public Action<Exception> OnException { get; set; }
        public Action<WebSocketState> OnStatusChange { get; set; }
        #endregion

        #region internal classes
        internal class UnTypedActionContainer
        {
            public Action<object> Action { get; set; }
            public Type ActionType { get; set; }
        }

        internal class DataCarrier
        {
            public string H { get; set; }
            public string M { get; set; }
            public string[] A { get; set; }

            public DataCarrier()
            {

            }
        }

        #endregion

        public SignalRHubClient(string Host, string HubName, bool useHttps, bool autoconnect = true)
        {
            _Host = Host;
            _UseHttps = useHttps;
            _HubName = HubName;
            if (useHttps) { httphost = "https://" + Host; wshost = "wss://" + Host; } else { httphost = "http://" + Host; wshost = "ws://" + Host; }
            _actionMap = new Dictionary<string, UnTypedActionContainer>();
            if (autoconnect) this.Connect();
        }

        #region Public Methods
        public bool Connect()
        {
            try
            {
                HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(httphost + "negotiate?connectionData=%5B%7B%22name%22%3A%22" + _HubName.ToLower() + "%22%7D%5D&clientProtocol=1.3&_=1408716619953");
                var response = (HttpWebResponse)webRequest.GetResponse();

                using (var sr = new StreamReader(response.GetResponseStream()))
                {
                    _NegotiatedResponse = JsonConvert.DeserializeObject<NegotiateResponse>(sr.ReadToEnd());
                    _connectionToken = Uri.EscapeDataString(_NegotiatedResponse.ConnectionToken);
                }
                if (string.IsNullOrEmpty(_connectionToken) || _NegotiatedResponse.TryWebSockets == false) return (false);
                return (true);
            }
            catch (Exception E)
            {
                if (OnException != null) OnException(E);
                throw (E);
            }
        }

        public void Open()
        {
            Random r = new Random((int)DateTime.UtcNow.Ticks);
            if (_ws == null)
            {
                _ws = new WebSocket(wshost + "connect?transport=webSockets&connectionToken=" + _connectionToken + "&connectionData=%5B%7B%22name%22%3A%22" + _HubName.ToLower() + "%22%7D%5D&tid=" + r.Next(10).ToString());
            }
            else
            {
                _ws = new WebSocket(wshost + "reconnect?transport=webSockets&connectionToken=" + _connectionToken + "&connectionData=%5B%7B%22name%22%3A%22" + _HubName.ToLower() + "%22%7D%5D&tid=" + r.Next(10).ToString());
            }
            AttachAndConnect();
        }

        public void Invoke(string method, string data)
        {
            try
            {
                _SentMessages++;
                DataCarrier dataCarrier = new DataCarrier()
                {
                    H = _HubName,
                    M = method,
                    A = new string[] { data }
                };
                string wsPacket = JsonConvert.SerializeObject(dataCarrier);
                this._ws.Send(wsPacket);
            }
            catch (Exception E)
            {
                if (OnException != null) OnException(E);
                throw (E);
            }
        }

        public void On<T>(string method, Action<T> callback) where T : class
        {
            _actionMap.Add(method, new UnTypedActionContainer
            {
                Action = new Action<object>(x =>
                {
                    callback(x as T);
                }),
                ActionType = typeof(T)
            });
        }

        #endregion

        #region Private methods
        private void AttachAndConnect()
        {
            _ws.OnClose += _ws_OnClose;
            _ws.OnError += _ws_OnError;
            _ws.OnMessage += _ws_OnMessage;
            _ws.OnOpen += _ws_OnOpen;
            _ws.Connect();
        }

        void _ws_OnOpen(object sender, EventArgs e)
        {
            if (OnStatusChange != null) OnStatusChange(WebSocketState.Open);
            //UnityEngine.Debug.Log("Opened Connection");
        }

        void _ws_OnMessage(object sender, MessageEventArgs e)
        {
            _RecievedMessages++;
            try
            {
                if (e.Data.Contains("\"H\":\"" + _HubName + "\""))
                {
                    _RecievedHubMessages++;
                    var msgWrapper = JsonConvert.DeserializeObject<MessageWrapper>(e.Data).M[0];
                    _actionMap[msgWrapper.M].Action(msgWrapper.A[0]);
                }
            }
            catch (Exception E)
            {
                _RecievedHubErrors++;
                throw (E);
            }
        }

        void _ws_OnError(object sender, WebSocketSharp.ErrorEventArgs e)
        {
            if (OnException != null) OnException(e.Exception);
        }

        void _ws_OnClose(object sender, CloseEventArgs e)
        {
            if (OnStatusChange != null) OnStatusChange(WebSocketState.Closed);
            //UnityEngine.Debug.Log(e.Reason + " Code: " + e.Code + " WasClean: " + e.WasClean);
        }
        #endregion
    }
}
