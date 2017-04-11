using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
namespace GameFabric.Models
{
    [DataContract]
    public class GateResponse
    {
        #region Creators
        public GateResponse()
        {

        }

        public GateResponse(GateRequest CreateForRequest)
        {
            this.Id = CreateForRequest.Id;
            this.Created = CreateForRequest.Created;
            this.Kind = CreateForRequest.Kind;
        }
        public GateResponse(int aKind, int aResult, Guid aUserID,string payload)
        {
            Kind = aKind;
            ResultCode = aResult;
            UserId = aUserID;
            JsonPayload = payload;
        }
        #endregion

        [DataMember]
        public Guid Id { get; set; }
        [DataMember]
        public Guid UserId { get; set; }
        [DataMember]
        public int Kind { get; set; }
        [DataMember]
        public string JsonPayload { get; set; }
        [DataMember]
        public bool isCompressed { get; set; }
        [DataMember]
        public int ResultCode { get; set; }
        [DataMember]
        public long Created { get; set; }
        [DataMember]
        public long TimeTaken { get; set; }
    }
}
