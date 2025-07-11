using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;

public class ColorChange7Shades : MonoBehaviour, IMaterialModifier
{
    public bool IgnoreSpriteParent = false;
    public Color Grey;
    [Range(1.0f, 5.0f)]
    public float GreyBrightness = 2f;
    public Color Brown;
    [Range(1.0f, 5.0f)]
    public float BrownBrightness = 0.75f;
    [Range(1.0f, 5.0f)]
    public float BrownDarkness = 0.25f;
    public Color Red;
    [Range(1.0f, 5.0f)]
    public float RedBrightness = 0.75f;
    [Range(1.0f, 5.0f)]
    public float RedDarkness = 0.25f;
    public Color Green;
    [Range(1.0f, 5.0f)]
    public float GreenBrightness = 0.75f;
    [Range(1.0f, 5.0f)]
    public float GreenDarkness = 0.25f;
    public Color Blue;
    [Range(1.0f, 5.0f)]
    public float BlueBrightness = 0.75f;
    [Range(1.0f, 5.0f)]
    public float BlueDarkness = 0.25f;
    [HideInInspector]
    public Color SpecialSkinColor;
    [HideInInspector]
    public Color Skin;
    [Range(1.0f, 5.0f)]
    public float SkinBrightness = 0.75f;
    [Range(1.0f, 5.0f)]
    public float SkinDarkness = 0.25f;
    [HideInInspector]
    public Color Hair;
    [Range(1.0f, 5.0f)]
    public float HairBrightness = 0.75f;
    [Range(1.0f, 5.0f)]
    public float HairDarkness = 0.25f;
    [HideInInspector]
    public Color Eye;
    [Range(1.0f, 5.0f)]
    public float EyeBrightness = 0.75f;
    [Range(1.0f, 5.0f)]
    public float EyeDarkness = 0.25f;
    [HideInInspector]
    public string TextureName;

    private void OnValidate() {
        if (isActiveAndEnabled) {
            UpdateMaterialProperties();
        }
    }

    private void Awake() {
        if (isActiveAndEnabled) {
            UpdateMaterialProperties();
        }
    }

    public void UpdateMaterialProperties() {
        if (GetComponent<SpriteRenderer>() == null)
        {
            return;
        }
        Texture texture = GetComponent<SpriteRenderer>().sprite.texture;
        if (texture == null) {
            throw new MissingReferenceException(Utils.GetGameObjectPath(gameObject) + " Could not find texture for: " + GetComponent<SpriteRenderer>().sprite);
        }
        MaterialPropertyBlock mpb = new MaterialPropertyBlock();
        mpb.SetTexture("_MainTex", texture);
        mpb.SetColor("_Grey", Grey);
        mpb.SetFloat("_GreyBrightness", GreyBrightness);
        mpb.SetFloat("_GreyDarkness", GreyBrightness);
        mpb.SetColor("_Brown", Brown);
        mpb.SetFloat("_BrownBrightness", BrownBrightness);
        mpb.SetFloat("_BrownDarkness", BrownDarkness);
        mpb.SetColor("_Red", Red);
        mpb.SetFloat("_RedBrightness", RedBrightness);
        mpb.SetFloat("_RedDarkness", RedDarkness);
        mpb.SetColor("_Green", Green);
        mpb.SetFloat("_GreenBrightness", GreenBrightness);
        mpb.SetFloat("_GreenDarkness", GreenDarkness);
        mpb.SetColor("_Blue", Blue);
        mpb.SetFloat("_BlueBrightness", BlueBrightness);
        mpb.SetFloat("_BlueDarkness", BlueDarkness);
        mpb.SetColor("_Skin", Skin);
        mpb.SetColor("_Special_Skin_Color", SpecialSkinColor);
        mpb.SetFloat("_SkinBrightness", SkinBrightness);
        mpb.SetFloat("_SkinDarkness", SkinDarkness);
        mpb.SetColor("_Hair", Hair);
        mpb.SetFloat("_HairBrightness", HairBrightness);
        mpb.SetFloat("_HairDarkness", HairDarkness);
        mpb.SetColor("_Eye", Eye);
        mpb.SetFloat("_EyeBrightness", EyeBrightness);
        mpb.SetFloat("_EyeDarkness", EyeDarkness);
        GetComponent<SpriteRenderer>().SetPropertyBlock(mpb);
    }

    public Material GetModifiedMaterial(Material baseMaterial)
    {
        Texture texture = Resources.Load("Sprites/" + TextureName) as Texture;
        if (texture == null)
        {
            throw new MissingReferenceException("Could not find texture under directory: Sprites/" + TextureName);
        }
        baseMaterial.SetTexture("_MainTex", texture);
        baseMaterial.SetColor("_Grey", Grey);
        baseMaterial.SetFloat("_GreyBrightness", GreyBrightness);
        baseMaterial.SetFloat("_GreyDarkness", GreyBrightness);
        baseMaterial.SetColor("_Brown", Brown);
        baseMaterial.SetFloat("_BrownBrightness", BrownBrightness);
        baseMaterial.SetFloat("_BrownDarkness", BrownDarkness);
        baseMaterial.SetColor("_Red", Red);
        baseMaterial.SetFloat("_RedBrightness", RedBrightness);
        baseMaterial.SetFloat("_RedDarkness", RedDarkness);
        baseMaterial.SetColor("_Green", Green);
        baseMaterial.SetFloat("_GreenBrightness", GreenBrightness);
        baseMaterial.SetFloat("_GreenDarkness", GreenDarkness);
        baseMaterial.SetColor("_Blue", Blue);
        baseMaterial.SetFloat("_BlueBrightness", BlueBrightness);
        baseMaterial.SetFloat("_BlueDarkness", BlueDarkness);
        baseMaterial.SetColor("_Skin", Skin);
        baseMaterial.SetColor("_Special_Skin_Color", SpecialSkinColor);
        baseMaterial.SetFloat("_SkinBrightness", SkinBrightness);
        baseMaterial.SetFloat("_SkinDarkness", SkinDarkness);
        baseMaterial.SetColor("_Hair", Hair);
        baseMaterial.SetFloat("_HairBrightness", HairBrightness);
        baseMaterial.SetFloat("_HairDarkness", HairDarkness);
        baseMaterial.SetColor("_Eye", Eye);
        baseMaterial.SetFloat("_EyeBrightness", EyeBrightness);
        baseMaterial.SetFloat("_EyeDarkness", EyeDarkness);
        return baseMaterial;
    }
}