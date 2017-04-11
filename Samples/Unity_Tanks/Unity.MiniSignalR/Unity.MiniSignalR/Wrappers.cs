using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Unity.MiniSignalR
{
    public class MessageWrapper
    {
        public string C { get; set; }

        public DataWrapper[] M { get; set; }
    }

    public class DataWrapper
    {
        public string H { get; set; }

        public string M { get; set; }

        public string[] A { get; set; }
    }
}
