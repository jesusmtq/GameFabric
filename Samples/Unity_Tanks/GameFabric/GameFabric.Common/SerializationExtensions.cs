using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using GameFabric.Common.JSonConverters;

namespace GameFabric.Common
{
    public static class SerializationExtensions
    {
        public static string Serialize<T>(this T toSerialize)
        {
            return (JsonConvert.SerializeObject(toSerialize, new ActorIdConverter()));
        }

        public static T Deserialize<T>(this string toDeserialize)
        {
            return (JsonConvert.DeserializeObject<T>(toDeserialize, new ActorIdConverter()));
        }

        public static object Deserialize(this string toDeserialize, Type DeserializeAs)
        {
            return (JsonConvert.DeserializeObject(toDeserialize,DeserializeAs, new ActorIdConverter()));
        }
    }
}
