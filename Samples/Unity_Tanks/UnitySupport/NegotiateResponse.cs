using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Unity.MiniSignalR
{
    public class NegotiateResponse
    {
        public string Url { get; set; }

        public string ConnectionToken { get; set; }

        public string ConnectionId { get; set; }

        public decimal KeepAliveTimeout { get; set; }

        public decimal DisconnectTimeout { get; set; }

        public bool TryWebSockets { get; set; }

        public string ProtocolVersion { get; set; }

        public decimal TransportConnectTimeout { get; set; }
    }
}
