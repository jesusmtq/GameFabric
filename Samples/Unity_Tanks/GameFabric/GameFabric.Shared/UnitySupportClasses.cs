using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GameFabric.Shared
{
    //Unity support classes
    public class clientVector3
    {
        public float x { get; set; }
        public float y { get; set; }
        public float z { get; set; }

        public clientVector3()
        {

        }

        public clientVector3(float ax, float ay, float az)
        {
            x = ax;
            y = ay;
            z = az;
        }
    }

    public class clientQuaternion
    {
        public float x { get; set; }
        public float y { get; set; }
        public float z { get; set; }
        public float w { get; set; }

        public clientQuaternion()
        {

        }

        public clientQuaternion(float ax, float ay, float az, float aw)
        {
            x = ax;
            y = ay;
            z = az;
            w = aw;
        }
    }
}
