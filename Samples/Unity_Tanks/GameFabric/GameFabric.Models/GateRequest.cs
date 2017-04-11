using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace GameFabric.Models
{
    [DataContract]
    public class GateRequest
    {
        #region Creators
        public GateRequest()
        {
            Id = Guid.NewGuid();
            Kind = 0;
            JsonPayload = string.Empty;
            isCompressed = false;
            Created = DateTime.UtcNow.Ticks;
        }
        #endregion

        [DataMember]
        public Guid Id { get; set; }
        [DataMember]
        public Guid SessionId { get; set; }
        [DataMember]
        public int Kind { get; set; }
        [DataMember]
        public string JsonPayload { get; set; }
        [DataMember]
        public bool isCompressed { get; set; }
        [DataMember]
        public long Created { get; set; }

    }
}
