using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;

namespace GameFabric.Processors.Interfaces
{
    public interface IResponse
    {
        HttpStatusCode Status { get; set; }
    }
}
