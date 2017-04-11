using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameFabric.Services.Gateway.Support
{
    public class ConnectionLookup
    {
        private ConcurrentDictionary<string, HashSet<string>> userMapping;
        private ConcurrentDictionary<string, string> connectionMapping;
        public int NumUsers => (userMapping.Count);
        public int NumConnections => (connectionMapping.Count);

        public ConnectionLookup()
        {
            userMapping = new ConcurrentDictionary<string, HashSet<string>>();
            connectionMapping = new ConcurrentDictionary<string, string>();
        }

        public bool AddConnection(string userId, string connectionId)
        {
            //Add user fwd mapping
            HashSet<string> connset;
            if (!userMapping.TryGetValue(userId, out connset))
            {
                connset = new HashSet<string>();
                if (!userMapping.TryAdd(userId, connset)) return (false);
            }
            lock (connset)
            {
                connset.Add(connectionId);
            }
            var result = connectionMapping.TryAdd(connectionId, userId);
            //Add reverse mapping
            return (result);
        }

        public bool RemoveConnection(string connectionId)
        {
            string userId;
            if (connectionMapping.TryRemove(connectionId, out userId))
            {
                HashSet<string> connset;
                if (userMapping.TryGetValue(userId, out connset))
                {
                    var isLast = false;
                    lock (connset)
                    {
                        if (connset.Contains(connectionId)) connset.Remove(connectionId);
                        if (connset.Count == 0) isLast = true;
                    }
                    //Ok was last, remove the user mapping
                    var result = true;
                    if (isLast) result = userMapping.TryRemove(userId, out connset);
                    return (result);
                }
                return (false);
            }
            return (true);
        }

        public List<string> GetUserConnections(string userId)
        {
            HashSet<string> connset;
            if (userMapping.TryGetValue(userId, out connset))
            {
                List<string> connections;
                lock (connset)
                {
                    connections = connset.ToList();
                }
                return (connections);
            }
            return (new List<string>());
        }

        public string GetUserFromConnection(string connectionId)
        {
            string userid;
            return connectionMapping.TryGetValue(connectionId, out userid) ? (userid) : Guid.Empty.ToString();
        }
    }
}
