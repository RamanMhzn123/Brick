using System.Collections.Generic;
using UnityEngine;

namespace CardGame.Scripts.Core.Managers
{
    public class ObjectPoolManager : MonoBehaviour
    {
        public Transform poolParent;
        private readonly Dictionary<string, Queue<GameObject>> _pools = new();
        private readonly Dictionary<string, GameObject> _prefabs = new();

        protected void CreatePool(string poolKey, GameObject prefab, int poolSize)
        {
            if (_pools.ContainsKey(poolKey)) return;
            
            Queue<GameObject> queue = new Queue<GameObject>();
            
            for (int i = 0; i < poolSize; i++)
            {
                GameObject obj = Instantiate(prefab, poolParent);
                obj.SetActive(false);
                queue.Enqueue(obj);
            }
            
            _pools[poolKey] = queue;
            _prefabs[poolKey] = prefab;
        }

        protected GameObject GetObject(string poolKey)
        {
            if (!_pools.TryGetValue(poolKey, out Queue<GameObject> pool)) return null;
            
            if (pool.Count <= 0) return ExpandPool(poolKey);
                
            return _pools[poolKey].Dequeue();
        }

        private GameObject ExpandPool(string poolKey)
        {
            if (!_prefabs.TryGetValue(poolKey, out GameObject prefab)) return null;
            
            GameObject prefabGo = Instantiate(prefab, poolParent);
            prefabGo.SetActive(true);
            return prefabGo;
        }

        protected void ReturnObject(string poolKey, GameObject obj)
        {
            if (!_pools.ContainsKey(poolKey))
            {
                Destroy(obj);
                return;
            }

            obj.SetActive(false);
            _pools[poolKey].Enqueue(obj);
        }
    }
}