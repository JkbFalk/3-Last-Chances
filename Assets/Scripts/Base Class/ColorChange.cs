using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;

public class ColorChange : MonoBehaviour, IMaterialModifier
{
    public bool IgnoreSpriteParent = false;
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
    [HideInInspector]
    public Color SpecialSkinColor;
    [HideInInspector]
    public Color Skin;
    [HideInInspector]
    public float SkinBorder1 = 0.4f;
    [HideInInspector]
    public float SkinBorder2 = 0.8f;
    [HideInInspector]
    public Color Hair;
    [HideInInspector]
    public float HairBorder1 = 0.4f;
    [HideInInspector]
    public float HairBorder2 = 0.8f;
    [HideInInspector]
    public Color Eye;
    [HideInInspector]
    public float EyeBorder1 = 0.4f;
    [HideInInspector]
    public float EyeBorder2 = 0.8f;
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
        mpb.SetFloat("_Grey_Border_1", GreyBorder1);
        mpb.SetFloat("_Grey_Border_2", GreyBorder2);
        mpb.SetColor("_Brown", Brown);
        mpb.SetFloat("_Brown_Border_1", BrownBorder1);
        mpb.SetFloat("_Brown_Border_2", BrownBorder2);
        mpb.SetColor("_Red", Red);
        mpb.SetFloat("_Red_Border_1", RedBorder1);
        mpb.SetFloat("_Red_Border_2", RedBorder2);
        mpb.SetColor("_Green", Green);
        mpb.SetFloat("_Green_Border_1", GreenBorder1);
        mpb.SetFloat("_Green_Border_2", GreenBorder2);
        mpb.SetColor("_Blue", Blue);
        mpb.SetFloat("_Blue_Border_1", BlueBorder1);
        mpb.SetFloat("_Blue_Border_2", BlueBorder2);
        mpb.SetColor("_Skin", Skin);
        mpb.SetColor("_Special_Skin_Color", SpecialSkinColor);
        mpb.SetFloat("_Skin_Border_1", SkinBorder1);
        mpb.SetFloat("_Skin_Border_2", SkinBorder2);
        mpb.SetColor("_Hair", Hair);
        mpb.SetFloat("_Hair_Border_1", HairBorder1);
        mpb.SetFloat("_Hair_Border_2", HairBorder2);
        mpb.SetColor("_Eye", Eye);
        mpb.SetFloat("_Eye_Border_1", EyeBorder1);
        mpb.SetFloat("_Eye_Border_2", EyeBorder2);
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
        baseMaterial.SetFloat("_Grey_Border_1", GreyBorder1);
        baseMaterial.SetFloat("_Grey_Border_2", GreyBorder2);
        baseMaterial.SetColor("_Brown", Brown);
        baseMaterial.SetFloat("_Brown_Border_1", BrownBorder1);
        baseMaterial.SetFloat("_Brown_Border_2", BrownBorder2);
        baseMaterial.SetColor("_Red", Red);
        baseMaterial.SetFloat("_Red_Border_1", RedBorder1);
        baseMaterial.SetFloat("_Red_Border_2", RedBorder2);
        baseMaterial.SetColor("_Green", Green);
        baseMaterial.SetFloat("_Green_Border_1", GreenBorder1);
        baseMaterial.SetFloat("_Green_Border_2", GreenBorder2);
        baseMaterial.SetColor("_Blue", Blue);
        baseMaterial.SetFloat("_Blue_Border_1", BlueBorder1);
        baseMaterial.SetFloat("_Blue_Border_2", BlueBorder2);
        baseMaterial.SetColor("_Skin", Skin);
        baseMaterial.SetColor("_Special_Skin_Color", SpecialSkinColor);
        baseMaterial.SetFloat("_Skin_Border_1", SkinBorder1);
        baseMaterial.SetFloat("_Skin_Border_2", SkinBorder2);
        baseMaterial.SetColor("_Hair", Hair);
        baseMaterial.SetFloat("_Hair_Border_1", HairBorder1);
        baseMaterial.SetFloat("_Hair_Border_2", HairBorder2);
        baseMaterial.SetColor("_Eye", Eye);
        baseMaterial.SetFloat("_Eye_Border_1", EyeBorder1);
        baseMaterial.SetFloat("_Eye_Border_2", EyeBorder2);
        return baseMaterial;
    }
}