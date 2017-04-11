using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Actors;
using Newtonsoft.Json;

namespace GameFabric.Common.JSonConverters
{
    public class ActorIdConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            writer.WriteValue((ActorId)value);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null) return null;

            while (reader.Read() && reader.TokenType != JsonToken.EndObject)
                if (reader.TokenType == JsonToken.PropertyName)
                {
                    var key = (string)reader.Value;
                    switch (key)
                    {
                        case "GuidId":
                            {
                                if (reader.Read())
                                {
                                    ActorId id = new ActorId(new Guid((string)reader.Value));
                                    while (reader.TokenType != JsonToken.EndObject && reader.Read()) ;
                                    return id;
                                }
                                return null;
                            }
                        case "LongId":
                            {
                                if (reader.Read())
                                {
                                    ActorId id = new ActorId((long)reader.Value);
                                    while (reader.TokenType != JsonToken.EndObject && reader.Read()) ;
                                    return id;
                                }
                                return null;
                            }
                        case "StringId":
                            {
                                if (reader.Read())
                                {
                                    ActorId id = new ActorId((string)reader.Value);
                                    while (reader.TokenType != JsonToken.EndObject && reader.Read()) ;
                                    return id;
                                }
                                return null;
                            }
                        default:
                            reader.Read();
                            break;
                    }
                }
            return null;
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(ActorId);
        }
    }
}
