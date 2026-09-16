using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PooledObject : MonoBehaviour {
    public string PoolKey;
}

public static class ObjectPool {
    private static readonly Dictionary<string, Stack<GameObject>> Pools = new Dictionary<string, Stack<GameObject>>();
    private static Transform _root;

    private static Transform Root {
        get {
            if (_root == null && GameController.Instance != null) {
                GameObject rootObject = new GameObject("ObjectPool");
                rootObject.transform.SetParent(GameController.Instance.transform, false);
                _root = rootObject.transform;
            }
            return _root;
        }
    }

    public static GameObject Spawn(string resourcePath, Transform parent) {
        GameObject instance = null;
        if (Pools.TryGetValue(resourcePath, out Stack<GameObject> stack)) {
            while (stack.Count > 0 && instance == null) {
                GameObject candidate = stack.Pop();
                if (candidate != null) {
                    instance = candidate;
                }
            }
        }
        if (instance == null) {
            GameObject prefab = ResourceCache.Load<GameObject>(resourcePath);
            if (prefab == null) {
                Debug.LogError("ObjectPool could not load prefab: " + resourcePath);
                return null;
            }
            instance = Object.Instantiate(prefab);
            PooledObject marker = instance.GetComponent<PooledObject>();
            if (marker == null) {
                marker = instance.AddComponent<PooledObject>();
            }
            marker.PoolKey = resourcePath;
        }
        else {
            TemporaryObject temporary = instance.GetComponent<TemporaryObject>();
            if (temporary != null) {
                temporary.ResetForPoolReuse();
            }
        }
        instance.transform.SetParent(parent, false);
        instance.SetActive(false);
        return instance;
    }

    public static void Release(GameObject instance) {
        if (instance == null) {
            return;
        }
        PooledObject marker = instance.GetComponent<PooledObject>();
        if (marker == null || string.IsNullOrEmpty(marker.PoolKey) || Root == null) {
            Object.Destroy(instance);
            return;
        }
        instance.SetActive(false);
        instance.transform.SetParent(Root, false);
        if (!Pools.TryGetValue(marker.PoolKey, out Stack<GameObject> stack)) {
            stack = new Stack<GameObject>();
            Pools[marker.PoolKey] = stack;
        }
        stack.Push(instance);
    }

    public static IEnumerator Warm(string resourcePath, int count) {
        yield return ResourceCache.LoadAsync<GameObject>(resourcePath);
        GameObject prefab = ResourceCache.Load<GameObject>(resourcePath);
        if (prefab == null || Root == null) {
            yield break;
        }
        if (!Pools.TryGetValue(resourcePath, out Stack<GameObject> stack)) {
            stack = new Stack<GameObject>();
            Pools[resourcePath] = stack;
        }
        while (stack.Count < count) {
            GameObject instance = Object.Instantiate(prefab, Root);
            instance.SetActive(false);
            PooledObject marker = instance.GetComponent<PooledObject>();
            if (marker == null) {
                marker = instance.AddComponent<PooledObject>();
            }
            marker.PoolKey = resourcePath;
            stack.Push(instance);
            yield return null;
        }
    }

    public static void Clear() {
        foreach (Stack<GameObject> stack in Pools.Values) {
            while (stack.Count > 0) {
                GameObject instance = stack.Pop();
                if (instance != null) {
                    Object.Destroy(instance);
                }
            }
        }
        Pools.Clear();
    }
}
