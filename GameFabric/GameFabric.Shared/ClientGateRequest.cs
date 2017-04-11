using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GameFabric.Shared
{
    public class ClientGateRequest
    {
    #region Creators
        public ClientGateRequest()
        {
            Id = Guid.NewGuid();
            Kind = 0;
            JsonPayload = string.Empty;
            isCompressed = false;
            Created = DateTime.UtcNow.Ticks;
        }
        #endregion

        public Guid Id { get; set; }
        public Guid SessionId { get; set; }
        public int Kind { get; set; }
        public string JsonPayload { get; set; }
        public bool isCompressed { get; set; }
        public long Created { get; set; }
    }
}
