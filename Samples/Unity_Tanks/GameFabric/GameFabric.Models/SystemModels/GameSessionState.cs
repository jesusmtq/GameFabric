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
    public class GameSessionState : IExtensibleDataObject
    {
        [DataMember]
        public List<TankSessionMemberItem> Players { get; set; }
        [DataMember]
        public int PlayerOneScore { get; set; }
        [DataMember]
        public int PlayerTwoScore { get; set; }
        [DataMember]
        public int TotalRounds { get; set; }
        [DataMember]
        public bool isCreated { get; set; }
        public ExtensionDataObject ExtensionData { get; set; }
        public GameSessionState()
        {
            isCreated = false;
            Players = new List<TankSessionMemberItem>();
        }
    }
}
