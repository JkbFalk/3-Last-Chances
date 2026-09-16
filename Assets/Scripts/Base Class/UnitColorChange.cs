using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Unit;

public class UnitColorChange : MonoBehaviour {
    public Color SpecialSkinColor;
    public Color Skin;
    [Range(0f, 1.0f)]
    public float SkinBorder1 = 0.4f;
    [Range(0f, 1.0f)]
    public float SkinBorder2 = 0.8f;
    public Color Hair;
    [Range(0f, 1.0f)]
    public float HairBorder1 = 0.4f;
    [Range(0f, 1.0f)]
    public float HairBorder2 = 0.8f;
    public Color Eye;
    [Range(0f, 1.0f)]
    public float EyeBorder1 = 0.4f;
    [Range(0f, 1.0f)]
    public float EyeBorder2 = 0.8f;

    private Unit _unit;

    public void OnValidate() {
        UpdateMaterialProperties();
    }

    public void UpdateMaterialProperties()
    {
        if (_unit == null)
        {
            _unit = GetComponent<Unit>();
        }
        if (_unit.SpriteRenderers == null || _unit.SpriteRenderers.Count == 0)
        {
            _unit.InitializeSpriteRenderers();
        }
        if (_unit.UnitColorChange == null)
        {
            _unit.UnitColorChange = this;
        }
        foreach (SpriteRendererInfo sr_info in _unit.SpriteRenderers.Values)
        {
            if (sr_info.ColorChange != null && sr_info.Name != "Consumable")
            {

                sr_info.ColorChange.SpecialSkinColor = SpecialSkinColor;
                sr_info.ColorChange.Skin = Skin;
                sr_info.ColorChange.SkinBorder1 = SkinBorder1;
                sr_info.ColorChange.SkinBorder2 = SkinBorder2;
                sr_info.ColorChange.Hair = Hair;
                sr_info.ColorChange.HairBorder1 = HairBorder1;
                sr_info.ColorChange.HairBorder2 = HairBorder2;
                sr_info.ColorChange.Eye = Eye;
                sr_info.ColorChange.EyeBorder1 = EyeBorder1;
                sr_info.ColorChange.EyeBorder2 = EyeBorder2;
                sr_info.ColorChange.UpdateMaterialProperties();
            }
        }
    }
}