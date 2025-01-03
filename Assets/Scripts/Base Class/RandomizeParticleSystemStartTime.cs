using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomizeParticleSystemStartTime : MonoBehaviour
{
    public float MinDelayInSeconds = 0;
    public float MaxDelayInSeconds = 5;
    void Start()
    {
        //gameObject.SetActive(false);
        //GameController.Instance.WaitAndRunMethod(UnityEngine.Random.Range(MinDelayInSeconds, MaxDelayInSeconds), Restart);
    }

    public void Restart() {
        //gameObject.SetActive(true);
    }

}
