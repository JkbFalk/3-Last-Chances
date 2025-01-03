using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CopyPosition : MonoBehaviour
{
    public Transform ObjectToCopyPositionFrom;

    public void LateUpdate()
    {
        transform.position = ObjectToCopyPositionFrom.position;
    }

}
