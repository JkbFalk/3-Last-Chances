using UnityEngine;
using UnityEngine.Pool;

public class PooledObject : MonoBehaviour 
{
    // The specific pool this object belongs to
    public IObjectPool<PooledObject> Pool { get; set; }
    
    [HideInInspector] 
    public TemporaryObject TempObject;

    private void Awake()
    {
        // Cache this once!
        TempObject = GetComponent<TemporaryObject>();
    }

    // Call this to return the object to the pool safely
    public void Release()
    {
        if (Pool != null)
        {
            Pool.Release(this);
        }
        else
        {
            // Fallback just in case it was instantiated outside the pool
            Destroy(gameObject);
        }
    }
}