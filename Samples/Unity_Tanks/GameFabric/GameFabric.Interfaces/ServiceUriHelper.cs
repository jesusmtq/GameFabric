using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using GameFabric.Common.Attributes;
namespace GameFabric.Interfaces
{
    /// <summary>
    /// Simple Uri resolver to map services -> Uri, build internal dictionary using reflection of the Salt.Interfaces assembly looking for
    /// Salt.Common.Attributes.FabricLocationAttribute attributes on interfaces and adds them
    /// </summary>
    public static class ServiceUriHelper
    {
        #region Private methods
        private static Dictionary<Type, Uri> _Resolver = null;
        private static void BuildDictionary()
        {
            Dictionary<Type, Uri> toBuild = new Dictionary<Type, Uri>();
            Assembly asm = Assembly.GetExecutingAssembly();
            try
            {
                Type[] types = asm.GetTypes();
                foreach (Type type in types)
                {
                    if (type.IsInterface)
                    {
                        if (type.GetCustomAttributes(typeof(FabricLocationAttribute), false).Length == 1)
                        {
                            FabricLocationAttribute attr = (FabricLocationAttribute)type.GetCustomAttributes(typeof(FabricLocationAttribute), false)[0];
                            if (!toBuild.ContainsKey(type)) toBuild.Add(type, attr.ServiceUri);
                        }
                    }
                }
                _Resolver = toBuild;
            }
            catch (Exception E)
            {
                throw (new Exception("Failed to load FabricLocations", E));
            }
        }
        #endregion

        public static Uri Resolve<T>()
        {
            if (_Resolver == null) ServiceUriHelper.BuildDictionary();
            if (_Resolver != null) if (!_Resolver.ContainsKey(typeof(T))) return (new Uri(""));
            return (_Resolver[typeof(T)]);
        }
    }
}
