using System;
using System.Diagnostics;
using System.Fabric;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Actors.Runtime;
using GameFabric.Logging;

namespace Actors.Gate
{
    internal static class Program
    {
        /// <summary>
        /// This is the entry point of the service host process.
        /// </summary>
        private static void Main()
        {
            try
            {
                
                ActorRuntime.RegisterActorAsync<Gate>(
                   (context, actorType) => new ActorService(context, actorType, null, null, null, new ActorServiceSettings
                   {
                       ActorGarbageCollectionSettings = new ActorGarbageCollectionSettings(120, 60),
                       ActorConcurrencySettings = new ActorConcurrencySettings
                       {
                           LockTimeout = TimeSpan.FromSeconds(10)
                       }
                   })).GetAwaiter().GetResult();
                Thread.Sleep(Timeout.Infinite);
            }
            catch (Exception e)
            {
                ActorEventSource.Current.ActorHostInitializationFailed(e.ToString());
                e.Log();
                throw;
            }
        }
    }
}
