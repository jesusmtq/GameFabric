using System.Collections.Generic;
using System;
using System.Web.Http;
using Microsoft.ServiceFabric.Actors;
using SeaBreeze.Common;
using SeaBreeze.Interfaces;
using SeaBreeze.Models;
using System.Threading.Tasks;
using SeaBreeze.Shared;
using SeaBreeze.Common.Hashes;
using SeaBreeze.Services.Gateway.Filters;
using SeaBreeze.Shared.Requests;
using SeaBreeze.Shared.Responses;

namespace Services.Gateway.Controllers
{
    public class AdminController: ApiController
    {
        [Route("admin/checkuser/{name}")]
        [HttpGet]
        public async Task<bool> CheckUser(string name)
        {
            try
            {
                ActorId id = new ActorId(1);
                var gateproxy = id.Proxy<IGate>();
                GateRequest request = new GateRequest();
                UserExistsRequest uer = new Shared.Requests.UserExistsRequest();
                uer.UserName = name;
                request.Kind = 1;
                request.JsonPayload = uer.Serialize();
                GateResponse response = await gateproxy.Process(request);

                if(response.ResultCode==200)
                {
                    UserExistsResponse r = response.JsonPayload.Deserialize<UserExistsResponse>();
                    return (r.Exists);
                }

                return (false);
            }
            catch (Exception e)
            {
                await Logger.LogMessage(e);
                throw (e);
            }
        }
    }
}
