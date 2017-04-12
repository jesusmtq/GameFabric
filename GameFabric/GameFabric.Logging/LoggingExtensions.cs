using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Runtime;
namespace GameFabric.Logging
{
    //public enum LogLevel:int { Informational=0,Warning=1,Error=2,Critical=3,Debug=4}
    public static class LoggingExtensions
    {
        public static void Log(this string Message)
        {
            Logger.Instance.Message(Message);
        }

        public static void Log(this string Message, string data)
        {
            Logger.Instance.MessageData(Message, data);
        }


        public static void Log(this Exception E)
        {
            Logger.Instance.ExceptionMessage(E.FormatException());
        }
        public static void Log(this Actor actor, string Message)
        {
            Logger.Instance.ActorMessage(actor, "{0}", Message);
        }
        public static void Log(this Actor actor, Exception e)
        {
            Logger.Instance.ExceptionActorMessage(actor, "{0}", e.FormatException());
        }

        #region Exception formatter

        public static string FormatException(this Exception E)
        {
            System.Text.StringBuilder sb = new StringBuilder();
            sb.Append(E.Message);
            sb.Append("\r\n");
            sb.Append(E.Source);
            sb.Append("\r\n");
            sb.Append(E.StackTrace);
            sb.Append("\r\n");
            if (E.InnerException != null) sb.Append(E.InnerException.FormatException());
            sb.Append("\r\n");
            return (sb.ToString());
        }
        #endregion

        #region Debug loggers

        public static void LogDebug(this string Message)
        {
#if DEBUG
            Logger.Instance.Message(Message);
#endif
        }

        public static void LogDebug(this Actor actor, string Message)
        {
#if DEBUG
            Logger.Instance.ActorMessage(actor, "{0}", Message);
#endif
        }
        public static void LogDebug(this string Message, string parameterName, string paramterData)
        {
#if DEBUG
            Logger.Instance.ParameterData(Message, parameterName, paramterData);
#endif
        }
        #endregion

    }
}
