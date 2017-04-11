using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GameFabric.Processors.Interfaces;

namespace GameFabric.Shared.Requests
{
    public class LoginUserRequest: IRequest
    {
        public string UserName { get; set; }
        public string Password { get; set; }
    }

}
