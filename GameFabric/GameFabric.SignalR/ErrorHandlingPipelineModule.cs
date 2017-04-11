using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR.Hubs;

namespace GameFabric.SignalR
{
    public class ErrorHandlingPipelineModule : HubPipelineModule
    {
        protected override async void OnIncomingError(ExceptionContext exceptionContext, IHubIncomingInvokerContext invokerContext)
        {
            try
            {
                //await Logger.Instance.Error<ErrorHandlingPipelineModule>(exceptionContext.Error);
            }
            catch (Exception)
            {

            }
            base.OnIncomingError(exceptionContext, invokerContext);
        }
    }
}
