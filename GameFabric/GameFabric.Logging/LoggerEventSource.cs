using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics.Tracing;
using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Runtime;
namespace GameFabric.Logging
{
    [EventSource(Name = "GameFabric-ServiceFabric-Logger")]
    public sealed class Logger : EventSource
    {
        public static readonly Logger Instance = new Logger();
        static Logger()
        {
            // A workaround for the problem where ETW activities do not get tracked until Tasks infrastructure is initialized.
            // This problem will be fixed in .NET Framework 4.6.2.
            Task.Run(() => { });
        }

        private Logger() : base() { }

        #region Keywords
        // Event keywords can be used to categorize events. 
        // Each keyword is a bit flag. A single event can be associated with multiple keywords (via EventAttribute.Keywords property).
        // Keywords must be defined as a public class named 'Keywords' inside EventSource that uses them.
        public static class Keywords
        {
            public const EventKeywords HostInitialization = (EventKeywords)0x1L;
            public const EventKeywords GameFabricFramework = (EventKeywords)0x2L;
            public const EventKeywords GameFabricException = (EventKeywords)0x4L;
        }
        #endregion

        [NonEvent]
        public void Message(string message, params object[] args)
        {
            if (this.IsEnabled())
            {
                string finalMessage = string.Format(message, args);
                Message(finalMessage);
            }
        }

        private const int MessageEventId = 1;
        [Event(MessageEventId, Level = EventLevel.Informational, Message = "{0}")]
        public void Message(string message)
        {
            if (this.IsEnabled())
            {
                WriteEvent(MessageEventId, message);
            }
        }

        private const int MessageDataEventId = 2;
        [Event(MessageDataEventId, Level = EventLevel.Informational, Message = "{0}")]
        public void MessageData(string message, string data)
        {
            if (this.IsEnabled())
            {
                WriteEvent(MessageDataEventId, message, data);
            }
        }

        private const int MessageParameterEventId = 3;
        [Event(MessageParameterEventId, Level = EventLevel.Informational, Message = "{0}")]
        public void ParameterData(string Message, string parameterName, string paramterData)
        {
            if (this.IsEnabled())
            {
                WriteEvent(MessageParameterEventId, Message, parameterName, paramterData);
            }
        }



        private const int WarningMessageEventId = 4;
        [Event(WarningMessageEventId, Level = EventLevel.Warning, Message = "{0}")]
        public void WarningMessage(string message)
        {
            if (this.IsEnabled())
            {
                WriteEvent(WarningMessageEventId, message);
            }
        }

        private const int ExceptionMessageEventId = 5;
        [Event(ExceptionMessageEventId, Level = EventLevel.Error, Message = "{0}")]
        public void ExceptionMessage(string message)
        {
            if (this.IsEnabled())
            {
                WriteEvent(ExceptionMessageEventId, message);
            }
        }

        private const int CriticalMessageEventId = 6;
        [Event(CriticalMessageEventId, Level = EventLevel.Critical, Message = "{0}")]
        public void CriticalMessage(string message)
        {
            if (this.IsEnabled())
            {
                WriteEvent(CriticalMessageEventId, message);
            }
        }

        [NonEvent]
        public void ExceptionActorMessage(Actor actor, string message, params object[] args)
        {
            if (this.IsEnabled()
                && actor.Id != null
                && actor.ActorService != null
                && actor.ActorService.Context != null
                && actor.ActorService.Context.CodePackageActivationContext != null)
            {
                string finalMessage = string.Format(message, args);
                ExceptionActorMessageEvent(
                    actor.GetType().ToString(),
                    actor.Id.ToString(),
                    actor.ActorService.Context.CodePackageActivationContext.ApplicationTypeName,
                    actor.ActorService.Context.CodePackageActivationContext.ApplicationName,
                    actor.ActorService.Context.ServiceTypeName,
                    actor.ActorService.Context.ServiceName.ToString(),
                    actor.ActorService.Context.PartitionId,
                    actor.ActorService.Context.ReplicaId,
                    actor.ActorService.Context.NodeContext.NodeName,
                    finalMessage);
            }
        }

        [NonEvent]
        public void ActorMessage(Actor actor, string message, params object[] args)
        {
            if (this.IsEnabled()
                && actor.Id != null
                && actor.ActorService != null
                && actor.ActorService.Context != null
                && actor.ActorService.Context.CodePackageActivationContext != null)
            {
                string finalMessage = string.Format(message, args);
                ActorMessage(
                    actor.GetType().ToString(),
                    actor.Id.ToString(),
                    actor.ActorService.Context.CodePackageActivationContext.ApplicationTypeName,
                    actor.ActorService.Context.CodePackageActivationContext.ApplicationName,
                    actor.ActorService.Context.ServiceTypeName,
                    actor.ActorService.Context.ServiceName.ToString(),
                    actor.ActorService.Context.PartitionId,
                    actor.ActorService.Context.ReplicaId,
                    actor.ActorService.Context.NodeContext.NodeName,
                    finalMessage);
            }
        }

        private const int ActorMessageEventId = 7;
        [Event(ActorMessageEventId, Level = EventLevel.Informational, Message = "{9}")]
        private
            void ActorMessage(
            string actorType,
            string actorId,
            string applicationTypeName,
            string applicationName,
            string serviceTypeName,
            string serviceName,
            Guid partitionId,
            long replicaOrInstanceId,
            string nodeName,
            string message)
        {

            WriteEvent(
                    ActorMessageEventId,
                    actorType,
                    actorId,
                    applicationTypeName,
                    applicationName,
                    serviceTypeName,
                    serviceName,
                    partitionId,
                    replicaOrInstanceId,
                    nodeName,
                    message);
        }

        private const int ExceptionActorMessageEventId = 8;
        [Event(ExceptionActorMessageEventId, Level = EventLevel.Error, Message = "{9}")]
        private
            void ExceptionActorMessageEvent(
            string actorType,
            string actorId,
            string applicationTypeName,
            string applicationName,
            string serviceTypeName,
            string serviceName,
            Guid partitionId,
            long replicaOrInstanceId,
            string nodeName,
            string message)
        {

            WriteEvent(
                    ExceptionActorMessageEventId,
                    actorType,
                    actorId,
                    applicationTypeName,
                    applicationName,
                    serviceTypeName,
                    serviceName,
                    partitionId,
                    replicaOrInstanceId,
                    nodeName,
                    message);
        }

    }
}
