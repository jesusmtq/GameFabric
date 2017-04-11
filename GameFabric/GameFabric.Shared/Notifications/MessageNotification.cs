using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GameFabric.Processors.Interfaces;

namespace GameFabric.Shared.Notifications
{
    public class MessageNotification:INotification
    {
        public string TextMessage { get; set; }

        public MessageNotification()
        {
            TextMessage = string.Empty;
        }

        public MessageNotification(string aMessage):this()
        {
            TextMessage = aMessage;
        }
    }
}
