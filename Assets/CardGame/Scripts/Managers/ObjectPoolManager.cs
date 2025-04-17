using System.Collections.Generic;
using UnityEngine;

namespace CardGame.Scripts.Managers
{
    public class ObjectPoolManager<T> : MonoBehaviour where T : Component
    {
        private readonly Dictionary<string, Queue<GameObject>> _pools = new();
        private readonly Dictionary<string, GameObject> _prefabs = new();

        protected void CreatePool(string poolKey, T prefab, int poolSize)
        {
            if (_pools.ContainsKey(poolKey)) return;
            
            GameObject original = prefab.gameObject;
            Queue<GameObject> queue = new Queue<GameObject>();
            
            for (int i = 0; i < poolSize; i++)
            {
                GameObject obj = Instantiate(original, transform);
                obj.SetActive(false);
                queue.Enqueue(obj);
            }
            
            _pools[poolKey] = queue;
            _prefabs[poolKey] = original;
        }

        protected T GetObject(string poolKey)
        {
            if (_pools.ContainsKey(poolKey))
            {
                if (_pools[poolKey].Count > 0)
                {
                    GameObject obj = _pools[poolKey].Dequeue();
                    obj.SetActive(true);
                    return obj.GetComponent<T>();
                }

                Debug.LogWarning($"Pool '{poolKey}' is empty. Dynamically expanding the pool.");
                return ExpandPool(poolKey);
            }

            Debug.LogError($"Pool '{poolKey}' does not exist.");
            return null;
        }

        private T ExpandPool(string poolKey)
        {
            if (_prefabs.ContainsKey(poolKey))
            {
                GameObject prefab = _prefabs[poolKey];
                GameObject obj = Instantiate(prefab, transform);
                obj.SetActive(true);
                return obj.GetComponent<T>();
            }

            Debug.LogError($"Cannot expand pool '{poolKey}'. Prefab not found.");
            return null;
        }

        protected void ReturnObject(string poolKey, GameObject obj)
        {
            if (!_pools.ContainsKey(poolKey))
            {
                Debug.Log($"Pool '{poolKey}' does not exist. Cannot return object.");
                Destroy(obj);
                return;
            }

            obj.SetActive(false);
            _pools[poolKey].Enqueue(obj);
        }
    }
}