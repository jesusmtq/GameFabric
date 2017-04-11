using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using GameFabric.Processors.Interfaces;
using GameFabric.Common.Attributes;
using GameFabric.Common;
namespace GameFabric.Processors
{
    public static class ProcessorHelper
    {
        private static Dictionary<int, ResolverEntry> _Resolver = null;

        /// <summary>
        /// Store attribute information for each processor
        /// </summary>
        internal class ResolverEntry
        {
            public Type ProcessorType { get; set; }
            public Type RequestType { get; set; }
            public Type ResponseType { get; set; }
            public ResolverEntry()
            {

            }

            public ResolverEntry(Type Processor,Type Request,Type Response)
            {
                ProcessorType = Processor;
                RequestType = Request;
                ResponseType = ResponseType;
            }
        }
        /// <summary>
        /// Build the dictionary containing the processors found using reflection
        /// </summary>
        private static void BuildDictionary()
        {
            Dictionary<int, ResolverEntry> toBuild = new Dictionary<int, ResolverEntry>();
            Assembly asm = Assembly.GetExecutingAssembly();
            try
            {
                Type[] types = asm.GetTypes();
                foreach (Type type in types)
                {
                    if (type.IsClass && type.GetInterface("IRequestProcessor") != null)
                    {
                        if (type.GetCustomAttributes(typeof(ReqestProcessorAttribute), false).Length == 1)
                        {
                            ReqestProcessorAttribute attr = (ReqestProcessorAttribute)type.GetCustomAttributes(typeof(ReqestProcessorAttribute), false)[0];
                            if (!toBuild.ContainsKey(attr.Identifier))
                            {
                                toBuild.Add(attr.Identifier,new ResolverEntry(type,attr.RequestType,attr.ResponseType));
                            }
                            else
                            {
                                throw (new ArgumentException($"Duplicate RequestProcessor identifier {attr.Identifier} for processor {attr.Name} found!"));
                            }
                        }
                    }
                }
                _Resolver = toBuild;
            }
            catch (Exception E)
            {
                throw (new Exception("Failed to load RequestProcessors", E));
            }
        }
        /// <summary>
        /// Resolve processor type for Id
        /// </summary>
        /// <param name="ProcessorId"></param>
        /// <returns></returns>
        public static Type ResolveRequestProcessorType(int ProcessorId)
        {
            if (_Resolver == null) ProcessorHelper.BuildDictionary();
            if (!_Resolver.ContainsKey(ProcessorId)) throw (new ArgumentOutOfRangeException($"Cannot resolve processor with id {ProcessorId}"));
            return (_Resolver[ProcessorId].ProcessorType);
        }
        /// <summary>
        /// Resolve Request type for processorId
        /// </summary>
        /// <param name="ProcessorId"></param>
        /// <returns></returns>
        public static Type ResolveRequestProcessorRequestType(int ProcessorId)
        {
            if (_Resolver == null) ProcessorHelper.BuildDictionary();
            if (!_Resolver.ContainsKey(ProcessorId)) throw (new ArgumentOutOfRangeException($"Cannot resolve processor with id {ProcessorId}"));
            return (_Resolver[ProcessorId].RequestType);
        }
        /// <summary>
        /// Resolve Response type for ProcessorId
        /// </summary>
        /// <param name="ProcessorId"></param>
        /// <returns></returns>
        public static Type ResolveRequestProcessorResponseType(int ProcessorId)
        {
            if (_Resolver == null) ProcessorHelper.BuildDictionary();
            if (!_Resolver.ContainsKey(ProcessorId)) throw (new ArgumentOutOfRangeException($"Cannot resolve processor with id {ProcessorId}"));
            return (_Resolver[ProcessorId].ResponseType);
        }
        /// <summary>
        /// Test if a specific ProcessorId is available
        /// </summary>
        /// <param name="ProcessorId"></param>
        /// <returns></returns>
        public static bool CanProcess(int ProcessorId)
        {
            if (_Resolver == null) ProcessorHelper.BuildDictionary();
            return (_Resolver.ContainsKey(ProcessorId));
        }
        /// <summary>
        /// Get instance of Processor for Id
        /// </summary>
        /// <param name="ProcessorId"></param>
        /// <returns></returns>
        public static IRequestProcessor GetProcessor(int ProcessorId)
        {
            Type ptype = ProcessorHelper.ResolveRequestProcessorType(ProcessorId);
            return (IRequestProcessor)Activator.CreateInstance(ptype);
        }

    }
}
