using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameFabric.Processors.Interfaces
{
    public interface IRequestProcessor
    {
        int ProcessorId { get;}
        bool Authenticated { get; }
        Task<IResponse> Process(IRequest request);
    }
}
