using Unity.VisualScripting;
using UnityEngine;

public class DestroyGameObjectAfterGivenTime : MonoBehaviour {
    public float DestroyAfterSeconds = 1;
    public float DestroyTime = 1;
    public bool DetachIfParentDestroyed = true;
    private float _counter = 0;

    private void Update()
    {
        _counter += Time.deltaTime;
        if(_counter >= DestroyAfterSeconds)
        {
            if(GetComponent<TemporaryObject>() != null && DestroyTime > 0 && gameObject.IsDestroyed() == false) {
                GetComponent<TemporaryObject>().MakeObjectDisappear(DestroyTime);
            }
            else {
                Destroy(gameObject);
            }
        }
    }
}