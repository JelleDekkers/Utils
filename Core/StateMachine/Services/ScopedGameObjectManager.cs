using System.Collections.Generic;
using UnityEngine;
using Utils.Core.Services;

namespace Utils.Core.Flow.Services
{
    /// <summary>
    /// Manager service for keepting track of spawned GameObjects and their users
    /// When adding a new scope for a prefab, if the prefab is not in <see cref="instanceScopeTable"/> a new instance will be created
    /// When removing a scope and the instance has no more users it will be destroyed
    /// </summary>
    public class ScopedGameObjectManager : IService
    {
        private class InstanceScope
        {
            public GameObject instance;
            public List<object> users;

            public InstanceScope(GameObject instance, object user)
            {
                this.instance = instance;
                users = new List<object>() { user };
            }
        }

        private Dictionary<int, InstanceScope> instanceScopeTable = new Dictionary<int, InstanceScope>();

        public GameObject AddScope(GameObject prefab, object user, out bool isNew)
        {
            GameObject instance;
            int prefabID = prefab.GetInstanceID();
            if (!instanceScopeTable.ContainsKey(prefabID))
            {
                instance = Object.Instantiate(prefab);
                isNew = true;
                instanceScopeTable.Add(prefabID, new InstanceScope(instance, user));
            }
            else
            {
                instanceScopeTable[prefabID].users.Add(user);
                isNew = false;
                instance = instanceScopeTable[prefabID].instance;
            }

            return instance;
        }

        public void RemoveScope(GameObject prefab, object user)
        {
            int prefabID = prefab.GetInstanceID();
            if (instanceScopeTable.ContainsKey(prefabID))
            {
                if (instanceScopeTable[prefabID].users.Contains(user))
                {
                    instanceScopeTable[prefabID].users.Remove(user);
                }

                if(instanceScopeTable[prefabID].users.Count == 0)
                {
                    Object.Destroy(instanceScopeTable[prefabID].instance);
                    instanceScopeTable.Remove(prefabID);
                }
            }
        }
    }
}
