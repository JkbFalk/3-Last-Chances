using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

/// <summary>
/// Caches Resources.Load results so combat/UI never hits disk after the first fetch.
/// Preload via LoadAsync during the loading screen. Swap the Load implementations
/// to Addressables later without changing call sites.
/// </summary>
public static class ResourceCache {
    private static readonly Dictionary<string, Object> Cache = new Dictionary<string, Object>();

    public static T Load<T>(string path) where T : Object {
        if (string.IsNullOrEmpty(path)) {
            return null;
        }
        if (Cache.TryGetValue(path, out Object cached) && cached != null) {
            return cached as T;
        }
        T loaded = Resources.Load<T>(path);
        if (loaded != null) {
            Cache[path] = loaded;
        }
        return loaded;
    }

    public static Object Load(string path) {
        return Load<Object>(path);
    }

    public static Sprite LoadSprite(string path) {
        return Load<Sprite>(path);
    }

    public static IEnumerator LoadAsync<T>(string path) where T : Object {
        if (string.IsNullOrEmpty(path)) {
            yield break;
        }
        if (Cache.TryGetValue(path, out Object cached) && cached != null) {
            yield break;
        }
        ResourceRequest request = Resources.LoadAsync<T>(path);
        yield return request;
        if (request.asset != null) {
            Cache[path] = request.asset;
        }
    }

    public static void Clear() {
        Cache.Clear();
    }
}
