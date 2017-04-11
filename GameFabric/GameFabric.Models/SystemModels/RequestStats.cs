using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace GameFabric.Models.SystemModels
{
    [DataContract]
    public class RequestStats
    {
        [DataMember]
        public int CallKind { get; set; }
        [DataMember]
        public int NumCalls { get; set; }
        [DataMember]
        public long TotalTime { get; set; }
        [DataMember]
        public long TotalSize { get; set; }
        [DataMember]
        public long CompressedSize { get; set; }

        public RequestStats()
        {
            CallKind=0;
            NumCalls = 0;
            TotalTime = 0;
            TotalSize = 0;
            CompressedSize = 0;
        }

        public RequestStats(int kind):this()
        {
            CallKind = kind;
        }

        public override string ToString()
        {
            System.Text.StringBuilder sb = new StringBuilder();
            sb.Append($"CallKind:{CallKind} ");
            sb.Append($"NumCalls:{NumCalls} ");
            sb.Append($"TotalTime:{TotalTime} ");
            sb.Append($"TotalSize:{TotalSize} ");
            sb.Append($"CompressedSize:{CompressedSize} ");
            return (sb.ToString());
        }
    }
}
