using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class FogOfWar : MonoBehaviour
{
    private Tilemap _tilemap;
    private void OnTriggerEnter2D(Collider2D other) {
        if(other.CompareTag("Hitbox") == false || other.GetComponentInParent<Player>() == null) {
            return;
        }
        _tilemap = GetComponent<Tilemap>();
        RevealFogOfWar();
    }

    public void RevealFogOfWar() {
        _tilemap.color = new Color(_tilemap.color.r, _tilemap.color.g, _tilemap.color.b, _tilemap.color.a - 0.02f);
        if(_tilemap.color.a <= 0) {
            gameObject.SetActive(false);
        }
        else {
            GameController.Instance.WaitAndRunMethod(0.01f, RevealFogOfWar);
        }
    }
}
