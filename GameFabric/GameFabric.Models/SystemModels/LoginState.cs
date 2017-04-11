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
    public class LoginState : IExtensibleDataObject
    {
        [DataMember]
        public ActorId UserActorId { get; set; }
        [DataMember]
        public string PasswordHash { get; set; }
        [DataMember]
        public DateTime Created { get; set; }
        [DataMember]
        public DateTime Used { get; set; }
        [DataMember]
        public int FailCount { get; set; }
        [DataMember]
        public DateTime LastFail { get; set; }
        public ExtensionDataObject ExtensionData { get; set; }

        public LoginState()
        {
            Created = DateTime.UtcNow;
            Used = Created;
            FailCount = 0;
        }
    }
}
