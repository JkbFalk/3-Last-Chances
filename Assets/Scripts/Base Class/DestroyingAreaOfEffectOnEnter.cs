using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyingAreaOfEffectOnEnter : MonoBehaviour
{ 

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (isActiveAndEnabled)
        {
            DestroyAreaOfEffect(other);
        }
    }

    private void DestroyAreaOfEffect(Collider2D other)
    {
        AreaOfEffect[] aoes = other.GetComponentsInChildren<AreaOfEffect>();
        if(aoes.Length != 0)
        {
            Unit unit = other.GetComponentInChildren<Unit>();
            if (unit == null)
            {
                unit = other.GetComponentInParent<Unit>();
            }
            if (unit == null)
            {
                foreach (AreaOfEffect aoe in aoes)
                {
                    aoe.IsDisappearing = true;
                    aoe.DisappearTimeInSeconds = 0.25f;
                    aoe.DealingDamage = false;
                }
            }
        }
    }
}
