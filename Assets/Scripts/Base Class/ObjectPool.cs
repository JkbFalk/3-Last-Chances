using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool; // Required for the built-in API

public static class ObjectPool 
{
    // Map an integer hash (for fast lookups) to the Unity Object Pool
    private static readonly Dictionary<int, IObjectPool<PooledObject>> Pools = new Dictionary<int, IObjectPool<PooledObject>>();
    private static Transform _root;

    private static Transform Root 
    {
        get 
        {
            if (_root == null && GameController.Instance != null) 
            {
                GameObject rootObject = new GameObject("ObjectPool");
                rootObject.transform.SetParent(GameController.Instance.transform, false);
                _root = rootObject.transform;
            }
            return _root;
        }
    }

    // Factory method to get or create a pool for a specific prefab path
    private static IObjectPool<PooledObject> GetOrCreatePool(string resourcePath)
    {
        int key = Animator.StringToHash(resourcePath);

        if (Pools.TryGetValue(key, out IObjectPool<PooledObject> existingPool))
        {
            return existingPool;
        }

        // We must declare the variable before using it in the closure so the spawned objects know their pool
        IObjectPool<PooledObject> newPool = null;

        newPool = new ObjectPool<PooledObject>(
            createFunc: () => 
            {
                // TELEMETRY: If we are creating an object during combat, the pool wasn't warmed enough!
                if (Player.HasInstance() && Player.Instance.InCombat)
                {
                    Debug.LogWarning($"[Pool Warning] Instantiating {resourcePath} mid-combat! Increase pool size.");
                }

                GameObject prefab = ResourceCache.Load<GameObject>(resourcePath);
                if (prefab == null) 
                {
                    Debug.LogError($"[ObjectPool] Could not load prefab at {resourcePath}");
                    return null;
                }

                GameObject go = Object.Instantiate(prefab, Root);
                PooledObject po = go.GetComponent<PooledObject>();
                if (po == null) po = go.AddComponent<PooledObject>();
                
                // Assign the pool reference so the object can release itself later
                po.Pool = newPool;
                return po;
            },
            actionOnGet: (po) => 
            {
                // Reset state when pulling from the pool
                if (po.TempObject != null) 
                {
                    po.TempObject.ResetForPoolReuse();
                }
            },
            actionOnRelease: (po) => 
            {
                // Deactivate and reparent when returning to the pool
                po.gameObject.SetActive(false);
                po.transform.SetParent(Root, false);
            },
            actionOnDestroy: (po) => 
            {
                // Called if the pool exceeds maxSize or is cleared
                Object.Destroy(po.gameObject);
            },
            collectionCheck: true, // Throws an error if you accidentally release an object twice!
            defaultCapacity: 10,
            maxSize: 500 // Prevents infinite memory leaks if a bug spawns thousands of objects
        );

        Pools[key] = newPool;
        return newPool;
    }

    public static GameObject Spawn(string resourcePath, Transform parent) 
    {
        var pool = GetOrCreatePool(resourcePath);
        PooledObject instance = pool.Get(); // This automatically calls createFunc and actionOnGet
        
        if (instance != null)
        {
            instance.transform.SetParent(parent, false);
            return instance.gameObject;
        }
        return null;
    }

    public static void Release(GameObject instance) 
    {
        if (instance == null) return;

        PooledObject po = instance.GetComponent<PooledObject>();
        if (po != null) 
        {
            po.Release(); // Use the self-releasing pattern
        }
        else 
        {
            Object.Destroy(instance);
        }
    }

    // Warming with Unity's built-in pool works by Getting objects, holding them, and Releasing them all at once.
    public static IEnumerator Warm(string resourcePath, int count) 
    {
        yield return ResourceCache.LoadAsync<GameObject>(resourcePath);
        
        var pool = GetOrCreatePool(resourcePath);
        List<PooledObject> tempStorage = new List<PooledObject>(count);

        // Get objects to force instantiation
        for (int i = 0; i < count; i++) 
        {
            tempStorage.Add(pool.Get());
            yield return null; // Spread instantiation across frames so the loading screen doesn't freeze
        }

        // Release them all back into the pool
        foreach (var obj in tempStorage) 
        {
            obj.Release();
        }
    }

    public static void Clear() 
    {
        // Calling Clear() on Unity's ObjectPool automatically invokes actionOnDestroy for all inactive objects
        foreach (var pool in Pools.Values) 
        {
            pool.Clear();
        }
        Pools.Clear();
    }
}