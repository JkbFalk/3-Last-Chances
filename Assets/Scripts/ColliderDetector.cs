using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColliderDetector : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other) {
        //Debug.Log("Collided with: " + other);
    }

    private void OnTriggerStay2D(Collider2D other) {
        //Debug.Log("Collided with: " + other);
    }
}
