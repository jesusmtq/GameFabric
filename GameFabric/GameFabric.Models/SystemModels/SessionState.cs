using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using Microsoft.ServiceFabric.Actors;

namespace GameFabric.Models.SystemModels
{
    [DataContract]
    public class SessionState : IExtensibleDataObject
    {
        [DataMember]
        public ActorId UserActorId { get; set; }
        [DataMember]
        public string SessionHash { get; set; }
        [DataMember]
        public string SessionKey { get; set; }
        [DataMember]
        public DateTime Created { get; set; }
        [DataMember]
        public DateTime Used { get; set; }
        [DataMember]
        public string DeviceIdString { get; set; }
        [DataMember]
        public DateTime ValidTo { get; set; }
        [DataMember]
        public bool isCreated { get; set; }
        public ExtensionDataObject ExtensionData { get; set; }
        public SessionState()
        {
            Created = DateTime.UtcNow;
            Used = Created;
            ValidTo = DateTime.UtcNow.AddYears(1);
            DeviceIdString = string.Empty;
            SessionKey = string.Empty;
            SessionHash = string.Empty;
        }
    }
}
