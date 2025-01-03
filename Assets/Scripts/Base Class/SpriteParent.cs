using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteParent : MonoBehaviour
{
    public Color Grey;
    public float GreyBorder1 = 0.4f;
    public float GreyBorder2 = 0.8f;
    public Color Brown;
    public float BrownBorder1 = 0.4f;
    public float BrownBorder2 = 0.8f;
    public Color Red;
    public float RedBorder1 = 0.4f;
    public float RedBorder2 = 0.8f;
    public Color Green;
    public float GreenBorder1 = 0.4f;
    public float GreenBorder2 = 0.8f;
    public Color Blue;
    public float BlueBorder1 = 0.4f;
    public float BlueBorder2 = 0.8f;

    public void OnValidate() {
        UpdateMaterialProperties();
    }

    public void UpdateMaterialProperties()
    {
        foreach (ColorChange color_change in GetComponentsInChildren<ColorChange>(true))
        {
            if(color_change.IgnoreSpriteParent == false) {
                color_change.Grey = Grey;
                color_change.GreyBorder1 = GreyBorder1;
                color_change.GreyBorder2 = GreyBorder2;
                color_change.Brown = Brown;
                color_change.BrownBorder1 = BrownBorder1;
                color_change.BrownBorder2 = BrownBorder2;
                color_change.Red = Red;
                color_change.RedBorder1 = RedBorder1;
                color_change.RedBorder2 = RedBorder2;
                color_change.Green = Green;
                color_change.GreenBorder1 = GreenBorder1;
                color_change.GreenBorder2 = GreenBorder2;
                color_change.Blue = Blue;
                color_change.BlueBorder1 = BlueBorder1;
                color_change.BlueBorder2 = BlueBorder2;
                color_change.UpdateMaterialProperties();
            }
        }
        DestructibleEnvironment dest = GetComponent<DestructibleEnvironment>();
        if(dest != null) {
            dest.DestructibleColor = Grey.GetHashCode() != 0 ? Grey : Brown;
        }
    }
}
