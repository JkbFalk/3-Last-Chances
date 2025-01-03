using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NonInteractableNPC : MonoBehaviour
{
    
    public Dictionary<String, Unit.SpriteRendererInfo> SpriteRenderers = new Dictionary<String, Unit.SpriteRendererInfo>();
    public string DefaultAnimation = null;

    void Start()
    {
        InitializeSpriteRenderers();
        Damage.DeactivateUnit(GetComponent<Unit>());
        if(DefaultAnimation != null && !String.IsNullOrWhiteSpace(DefaultAnimation)) {
            GetComponent<Animator>().Play(DefaultAnimation);
        }
        SpriteRenderers["Lower Body"].Bone.GetComponent<BoxCollider2D>().enabled = false;
        SpriteRenderers["Lower Body"].Bone.Find("Environment Collision").gameObject.SetActive(false);
    }

    public void InitializeSpriteRenderers() {
        foreach (SpriteRenderer sr in GetComponentsInChildren<SpriteRenderer>(true)) {
            Unit.SpriteRendererInfo sr_info = new Unit.SpriteRendererInfo(sr.name, sr);
            switch (sr_info.Name.Replace(" (Unused)", "")) {
                case "Right Hand": { sr_info.SortingOrder = 17; sr_info.IsInFront = true; break; }
                case "Right Arm": { sr_info.SortingOrder = 16; sr_info.IsInFront = true; break; }
                case "Consumable": { sr_info.SortingOrder = 15; sr_info.IsInFront = true; break; }
                case "Projectile": { sr_info.SortingOrder = 14; sr_info.IsInFront = true; break; }
                case "Heavy": { sr_info.SortingOrder = 13; sr_info.IsInFront = true; break; }
                case "Light Right": { sr_info.SortingOrder = 12; sr_info.IsInFront = true; break; }
                case "Ranged": { sr_info.SortingOrder = 11; sr_info.IsInFront = true; break; }
                case "Hair": { sr_info.SortingOrder = 10; sr_info.IsInFront = true; break; }
                case "Upper Body": { sr_info.SortingOrder = 9; sr_info.IsInFront = true; break; }
                case "Head": { sr_info.SortingOrder = 8; sr_info.IsInFront = true; break; }
                case "Right Foot": { sr_info.SortingOrder = 7; sr_info.IsInFront = true; break; }
                case "Right Leg": { sr_info.SortingOrder = 6; sr_info.IsInFront = true; break; }
                case "Lower Body": { sr_info.SortingOrder = 5; sr_info.IsInFront = true; break; }
                case "Left Foot": { sr_info.SortingOrder = 4; sr_info.IsInFront = false; break; }
                case "Left Leg": { sr_info.SortingOrder = 3; sr_info.IsInFront = false; break; }
                case "Left Hand": { sr_info.SortingOrder = 2; sr_info.IsInFront = false; break; }
                case "Left Arm": { sr_info.SortingOrder = 1; sr_info.IsInFront = false; break; }
                case "Light Left": { sr_info.SortingOrder = 0; sr_info.IsInFront = false; break; }
                default: { sr_info.SortingOrder = 100; break; }
            }
            sr_info.ColorChange = sr_info.SpriteRenderer.GetComponent<ColorChange>();
            if (!SpriteRenderers.ContainsKey(sr_info.Name.Replace(" (Unused)", "")) && sr_info.SortingOrder != 100) {
                SpriteRenderers.Add(sr_info.Name.Replace(" (Unused)", ""), sr_info);
            }
        }
    }

    public void SetFaceVariant(String variant)
    {
        if(SpriteRenderers != null && SpriteRenderers.ContainsKey("Head") && SpriteRenderers["Head"].SpriteResolver != null)
        {
            SpriteRenderers["Head"].SpriteResolver.SetCategoryAndLabel("Head", variant);
            SpriteRenderers["Head"].SpriteResolver.ResolveSpriteToSpriteRenderer();
        }
    }

    public void SetDefaultSortingOrder() {}

    public void ChanceToBlink() {}

}
