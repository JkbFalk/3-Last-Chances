using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WarpZone : MonoBehaviour
{
    public string TeleportArea;
    public bool StartWithDirectionFlipped = false;
    public string DestinationName; 
    
    private void OnTriggerEnter2D(Collider2D other) {
        if(GameController.Instance.GameplayMode == Constants.GameplayMode.Regular && other.CompareTag("Hitbox") == true && other is CapsuleCollider2D && other.GetComponentInParent<Player>() != null) {
            Utils.MoveIntoArea(false, TeleportArea);
            if(StartWithDirectionFlipped) {
                Utils.ShouldStartFlipped = true;
            }
            if(!string.IsNullOrWhiteSpace(DestinationName)) {
                Utils.DestinationName = DestinationName;
            }
        }
    }
}
