using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Actors;
using GameFabric.Interfaces;
using GameFabric.Common.Compression;
using GameFabric.Logging;

namespace GameFabric.Common
{
    public static class GenericStorageExtensions
    {
        public static async Task<bool> SaveGeneric<T>(this T toSave,ActorId targetId,bool compress=true)
        {
            try
            {
                if (compress)
                {
                    return(await targetId.Proxy<IGenericStorage>().SetDataAsync(toSave.Serialize<T>().Compress()));
                } 
                else
                {
                    return (await targetId.Proxy<IGenericStorage>().SetDataAsync(toSave.Serialize<T>()));
                }
            }
            catch (Exception E)
            {
                E.Log();
                throw (E);
            }
        }

        public static async Task<T> LoadGeneric<T>(this ActorId toLoadFrom, bool compressed = true)
        {
            try
            {
                if (compressed)
                {
                    string data = await toLoadFrom.Proxy<IGenericStorage>().GetDataAsync();
                    return (data.Decompress().Deserialize<T>());
                }
                else
                {
                    string data = await toLoadFrom.Proxy<IGenericStorage>().GetDataAsync();
                    return (data.Deserialize<T>());
                }
            }
            catch (Exception E)
            {
                E.Log();
                throw (E);
            }
        }

        public static async Task<bool>ClearGeneric<T>(this ActorId toClear)
        {
            try
            {
                return await toClear.Proxy<IGenericStorage>().DeleteDataAsync();
            }
            catch (Exception E)
            {
                E.Log();
                return (false);
            }
        }

        public static async Task<bool> DeleteGeneric<T>(this ActorId toDelete)
        {
            try
            {
                return await toDelete.DeleteActor<IGenericStorage>();
            }
            catch (Exception E)
            {
                E.Log();
                return (false);
            }
        }

    }
}
