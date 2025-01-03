using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FootstepsOverride : MonoBehaviour
{
    public Area.FootstepsType Footsteps;
    public int Priority = 0;
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Hitbox") == true && other is CapsuleCollider2D)
        {
            Unit u = other.GetComponentInParent<Unit>();
            if(u != null) {
                other.GetComponentInParent<Unit>().FootstepsOverride.Add(Footsteps);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Hitbox") == true && other is CapsuleCollider2D)
        {
            Unit u = other.GetComponentInParent<Unit>();
            if(u != null) {
                other.GetComponentInParent<Unit>().FootstepsOverride.Remove(Footsteps);
            }
        }
    }
}
