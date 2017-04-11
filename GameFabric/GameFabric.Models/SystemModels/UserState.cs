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
    public class UserState : IExtensibleDataObject
    {
        [DataMember]
        public bool isCreated { get; set; }
        [DataMember]
        public DateTime Created { get; set; }
        [DataMember]
        public DateTime LastUsed { get; set; }
        [DataMember]
        public ActorId LoginActorId { get; set; }
        [DataMember]
        public ActorId LastSessionId { get; set; }
        [DataMember]
        public string UserName { get; set; }
        [DataMember]
        public bool isDeveloper { get; set; }
        public ExtensionDataObject ExtensionData { get; set; }
        public UserState()
        {
            Created = DateTime.UtcNow;
            LastUsed = Created;
            UserName = string.Empty;
            isDeveloper = false;
            isCreated = false;
        }
    }
}
