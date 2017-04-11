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
    public class GenericStorageState : IExtensibleDataObject
    { 
        [DataMember]
        public string Data { get; set; }
        [DataMember]
        public DateTime Created { get; set; }
        [DataMember]
        public DateTime Changed { get; set; }
        [DataMember]
        public bool ContainsData { get; set; }
        public ExtensionDataObject ExtensionData { get; set; }
        public GenericStorageState()
        {
            Data = string.Empty;
            Created = DateTime.UtcNow;
            Changed = Created;
        }
    }
}
