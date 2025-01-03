using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetChildActiveAfterNSeconds : MonoBehaviour
{
    public string PathToChild;
    public float Seconds = 0;
    public bool SetActive = true;
    public float ChangeBackAfterNSeconds = 0;
    private float _elapsedSeconds = 0;

    void Update()
    {
        if (Seconds != 0 && string.IsNullOrEmpty(PathToChild) == false)
        {
            if(_elapsedSeconds >= Seconds)
            {
                if(transform.Find(PathToChild) != null)
                {
                    transform.Find(PathToChild).gameObject.SetActive(SetActive);
                }
                if(ChangeBackAfterNSeconds != 0)
                {
                    _elapsedSeconds = 0;
                    Seconds = ChangeBackAfterNSeconds;
                    SetActive = !SetActive;
                    ChangeBackAfterNSeconds = 0;
                }
            }
            else
            {
                _elapsedSeconds += Time.deltaTime;
            }
        }
    }
}
