using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Actors;
using GameFabric.Common;
using GameFabric.Interfaces;
using GameFabric.Models;

namespace GameFabric.Interfaces
{
    public interface IUserEvent : IActorEvents
    {
        void SendNotificationAsync(int kind,string payload);
        void SendGateResponseAsync(GateResponse aResponse);
    }
}
