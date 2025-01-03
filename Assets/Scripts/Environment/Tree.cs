using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tree : MonoBehaviour
{
    void Start()
    {
        float speed = UnityEngine.Random.Range(0.3f, 0.6f);
        foreach(Transform child in transform) {
            if(child.GetComponent<Animator>() != null) {
                child.GetComponent<Animator>().SetFloat("SwaySpeed", speed);
            }
        }
    }
}
