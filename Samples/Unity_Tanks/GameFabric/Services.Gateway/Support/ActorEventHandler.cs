using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameFabric.Interfaces;
using Microsoft.ServiceFabric.Actors;
using GameFabric.Common;
using GameFabric.Logging;

namespace GameFabric.Services.Gateway.Support
{
    public class ActorEventHandler : IUserEvent
    {
        private string userId;
        public ActorEventHandler(string userId)
        {
            this.userId = userId;
        }

        void IUserEvent.SendGateResponseAsync(Models.GateResponse aResponse)
        {
            try
            {
                SignalRUserMapper.Instance.SendEventByUserId(aResponse.UserId.ToString(), aResponse.Serialize()).Wait(500);
            }
            catch (Exception e)
            {
                e.Log();
            }
        }

        void IUserEvent.SendNotificationAsync(int kind, string payload)
        {
            try
            {

            }
            catch (Exception e)
            {
                e.Log();
            }
        }
    }
}
