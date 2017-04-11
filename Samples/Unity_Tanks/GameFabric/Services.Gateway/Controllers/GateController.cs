using System.Collections.Generic;
using System;
using System.Web.Http;
using Microsoft.ServiceFabric.Actors;
using GameFabric.Common;
using GameFabric.Interfaces;
using GameFabric.Models;
using System.Threading.Tasks;
using GameFabric.Shared;
using GameFabric.Common.Hashes;
using GameFabric.Services.Gateway.Filters;
using GameFabric.Services.Gateway.Hubs;
using GameFabric.Models.SystemModels;
using System.Linq;
using GameFabric.Logging;

namespace Services.Gateway.Controllers
{
    public class GateController : ApiController
    {
        [Route("gate/exec/")]
        [HttpPost]
        //[CompressFilter]
        public async Task<GateResponse> Exec([FromBody]string payload)
        {
            try
            {
                //Retrieve session id
                //if (Request.)
                return (new GateResponse());
            }
            catch (Exception e)
            {
                e.Log();
                throw(e);
            }
        }
        [Route("gate/calls/")]
        [HttpGet]
        public Task<string> GetCalls([FromBody]string payload)
        {
            string ret = string.Empty;
            try
            {
                ret = "Exec calls:" + InterfaceHub._NumCalls.ToString() + " Que calls:" + InterfaceHub._NumQues.ToString() + " Map Calls" + InterfaceHub._MapCount.ToString();
            }
            catch (Exception E)
            {
                E.Log();
                ret = E.FormatException();
            }
            return (Task.FromResult(ret));
        }

        [Route("gate/stats/")]
        [HttpGet]
        public async Task<List<RequestStats>> GetStats()
        {
            GameFabric.Logging.Logger.Instance.Message($"Called stats at{DateTime.UtcNow.ToUniversalTime()} ms: {DateTime.UtcNow.Millisecond}. ");
            ActorId StatId = new ActorId(1);
            List<RequestStats> _Stats = await StatId.Proxy<IStatistics>().GetStatistics("Gate");
            _Stats = _Stats.OrderBy(p => p.CallKind).ToList();
            return (_Stats);
        }

        [Route("gate/pinggw/")]
        [HttpGet]
        public async Task<string> pinggw()
        {
            try
            {
                GameFabric.Logging.Logger.Instance.Message($"Called pinggw at{DateTime.UtcNow.ToUniversalTime()} ms: {DateTime.UtcNow.Millisecond}. ");
                ActorId gwid = new ActorId(1);
                GateResponse r = await gwid.Proxy<IGate>().Process(new GateRequest());
                return (r.JsonPayload);
            }
            catch (Exception e)
            {
                e.Log();
                return (e.FormatException());
            }
            
        }
        [Route("gate/exception/")]
        [HttpGet]
        public async Task<string> exception()
        {
            Exception E = new Exception("TestException");
            E.Log();
            return (E.FormatException());
        }
    }
}
