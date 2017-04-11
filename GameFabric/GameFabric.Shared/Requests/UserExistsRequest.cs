using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GameFabric.Processors.Interfaces;

namespace GameFabric.Shared.Requests
{
    public class UserExistsRequest : IRequest
    {
        public string UserName { get; set; }
    }
}
