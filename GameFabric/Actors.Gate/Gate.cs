using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Runtime;
using Microsoft.ServiceFabric.Actors.Client;
using GameFabric.Interfaces;
using GameFabric.Models;
using GameFabric.Processors;
using GameFabric.Processors.Interfaces;
using GameFabric.Common;
using GameFabric.Models.SystemModels;
using GameFabric.Common.Compression;
using System.Net;
using GameFabric.Logging;

namespace Actors.Gate
{
    [StatePersistence(StatePersistence.None)]
    internal class Gate : Actor, IGate
    {

        #region private vars
        private Dictionary<int, RequestStats> _StatDict = null;
        private ActorId _AssociatedUserId = null;
        private bool _isAuthenticated;
        private int _ForbiddenCount = 0;
        private DateTime _LastForbiddenCall = DateTime.UtcNow;
        private IActorTimer _Timer;
        private DateTime _Activated;
        private DateTime _LastCall;
        #endregion

        public Gate(ActorService actorService, ActorId actorId)
            : base(actorService, actorId)
        {
            _StatDict = new Dictionary<int, RequestStats>();
            _isAuthenticated = false;
        }
        protected override Task OnActivateAsync()
        {
            ActorEventSource.Current.ActorMessage(this, "Actor activated.");
            _Activated = DateTime.UtcNow;
            //_Timer = RegisterTimer(NotificationCallback, null, TimeSpan.FromSeconds(30), TimeSpan.FromSeconds(30));
            this.Log(new Exception("TestException"));
            return (Task.FromResult(true));
        }

        protected override async Task OnDeactivateAsync()
        {
            if (_Timer != null)
            {
                UnregisterTimer(_Timer);
                _Timer = null;
            }
            ActorId StatId = new ActorId(1);
            await StatId.Proxy<IStatistics>().UpdateStats("Gate", _StatDict);
            await base.OnDeactivateAsync();
        }

        private async Task NotificationCallback(object arg)
        {
            if (_Timer != null)
            {
                UnregisterTimer(_Timer);
                _Timer = null;
            }

            try
            {

            }
            catch (Exception e)
            {
                this.Log(e);
            }
        }

        /// <summary>
        /// Main process request method
        /// </summary>
        /// <param name="Request"></param>
        /// <returns></returns>
        async Task<GateResponse> IGate.Process(GateRequest Request)
        {
            try
            {
                _LastCall = DateTime.UtcNow;
                //Start a timer to log request processing time etc.
                System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
                sw.Start();
                //Test for ping
                if (Request.Kind==(int)RequestProcessorEnum._System_PingGw)
                {
                    GateResponse pingResponse = new GateResponse(Request);
                    pingResponse.JsonPayload = new GameFabric.Shared.Responses.EmptyResponse().Serialize();
                    pingResponse.ResultCode = (int)System.Net.HttpStatusCode.OK;
                    return (pingResponse);
                }
                //retrieve a processor for the call
                IRequestProcessor processor = ProcessorHelper.GetProcessor(Request.Kind);
                //Just test for authentication
                if (processor.Authenticated && !_isAuthenticated)
                {
                    GateResponse errorResponse = new GateResponse(Request);
                    errorResponse.ResultCode = (int)HttpStatusCode.Forbidden;
                    _ForbiddenCount++;
                    _LastForbiddenCall = DateTime.UtcNow;
                    return (await Task.FromResult(errorResponse));
                }
                //Deserialize request
                string Payload = Request.JsonPayload;
                //Check for compression and decompress if needed
                if (Request.isCompressed) Payload = Payload.Decompress();
                //Then deserialize

                IRequest embeddedRequest = (IRequest)Payload.Deserialize(ProcessorHelper.ResolveRequestProcessorRequestType(Request.Kind));

                //Execute processor
                IResponse result = await processor.Process(embeddedRequest);

                // Create response
                GateResponse response = new GateResponse(Request);
                //Set the response data and compress if needed
                response.JsonPayload = result.Serialize();
                long uncompsize = response.JsonPayload.Length;
                if (response.JsonPayload.Length > 512) { response.JsonPayload = response.JsonPayload.Compress(); response.isCompressed = true; }
                long compressedsize = response.JsonPayload.Length;
                //Stop timer and write stats
                sw.Stop();
                //Write stats
                if (!_StatDict.ContainsKey(Request.Kind)) _StatDict.Add(Request.Kind, new RequestStats(Request.Kind));
                _StatDict[Request.Kind].NumCalls++;
                _StatDict[Request.Kind].TotalTime += sw.ElapsedMilliseconds;
                _StatDict[Request.Kind].CompressedSize += compressedsize;
                _StatDict[Request.Kind].TotalSize += uncompsize;
                //Finalize response
                response.TimeTaken = sw.ElapsedMilliseconds;
                //Set response code
                response.ResultCode = (int)System.Net.HttpStatusCode.OK;
                this.ApplicationName.LogDebug(processor.ProcessorId.ToString(), sw.ElapsedMilliseconds.ToString());
                return (response);
            }
            catch (Exception E)
            {
                this.Log(E);
                GateResponse errorResponse = new GateResponse(Request);
                errorResponse.ResultCode = (int)HttpStatusCode.InternalServerError;
                return (await Task.FromResult(errorResponse));
            }
        }
    }
}