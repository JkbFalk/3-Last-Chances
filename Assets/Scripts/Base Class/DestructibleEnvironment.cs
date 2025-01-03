using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Unity.VisualScripting;
using UnityEngine;

public class DestructibleEnvironment : MonoBehaviour
{
    public float BaseHitPoints = 20;
    public int LevelRequiredToDealDamage = 1;
    public float HitPoints;
    public DestructibleType AnimationType;
    public enum DestructibleType { Wood, Metal, Rock, Fire, None};
    public float DebrisScale = 1;
    public string ClassAndMethodOnDestroy;
    public string ClassAndMethodCheckIfDestructible;
    public DateTime LastTimeHitSoundWasPlayed;
    public Color DestructibleColor;
    public float DestructibleBorder1 = 0.4f;
    public float DestructibleBorder2 = 0.8f;

    public void Start()
    {
        HitPoints = BaseHitPoints * Utils.GetExpectedPowerForLevel(Area.ComponentInstance.Level);
        gameObject.tag = "Destructible";
        bool hasLoot = false;
        foreach(Transform child in transform) {
            if(child.gameObject.name.Contains("Loot")) {
                hasLoot = true;
                child.gameObject.SetActive(false);
            }
        }
        if(hasLoot) {
            GameObject indicator = MonoBehaviour.Instantiate(Resources.Load("Prefabs/Environment/Drop Indicator")) as GameObject;
            indicator.transform.SetParent(transform, true);
            indicator.transform.localPosition = new Vector3(0, 2);
        }
        ColorChange colorChange = GetComponent<ColorChange>();
        if(colorChange != null && DestructibleColor != colorChange.Grey && DestructibleColor != colorChange.Brown) {
            DestructibleColor = GetComponent<ColorChange>().Grey;
        }
    }

    public void DestroyObject() {
        Utils.CreateAuditLog("Destroying Destructible: " + gameObject.name);
        List<Transform> loot = new();
        foreach(Transform child in transform) {
            if(gameObject != null && !gameObject.IsDestroyed() && child.gameObject.name.Contains("Loot")) {
                loot.Add(child);
            }
        }
        foreach(Transform item in loot) {
            item.gameObject.SetActive(true);
            item.SetParent(transform.parent);
            item.gameObject.name = item.gameObject.name + " (" + gameObject.name + ")";
            item.position = new Vector3(transform.position.x + UnityEngine.Random.Range(0f, 0f), transform.position.y + UnityEngine.Random.Range(0f, 0f));
        }
        Transform onDestroy = Utils.GetOnDestroyObject(transform);
        if(gameObject != null && !gameObject.IsDestroyed() && onDestroy != null) {
            onDestroy.gameObject.SetActive(true);
            onDestroy.SetParent(transform.parent);
            onDestroy.gameObject.name = onDestroy.gameObject.name + " (" + gameObject.name + ")";
        }
        if(AnimationType != DestructibleType.None) {
            GameObject vfx = Utils.CreateVisualEffect(new (Player.Instance), AnimationType.ToString() + "Destructible", transform.position.x, transform.position.y);
            gameObject.tag = "Untagged";
            foreach(ParticleSystem ps in vfx.GetComponentsInChildren<ParticleSystem>(true)) {
                ParticleSystem.MainModule main = ps.main;
                ParticleSystem.MinMaxCurve curve = main.startSize;
                curve.constantMin *= DebrisScale * transform.localScale.x;
                curve.constantMax *= DebrisScale* transform.localScale.x;
                main.startSize = curve;
                if(DestructibleColor.GetHashCode() != 0) {
                    main.startColor = DestructibleColor;
                }
            }
        }
        if(!string.IsNullOrWhiteSpace(ClassAndMethodOnDestroy)) {
            MethodInfo method = Type.GetType(ClassAndMethodOnDestroy.Split(".")[0]).GetMethod(ClassAndMethodOnDestroy.Split(".")[1], BindingFlags.Public | BindingFlags.Static);
            method.Invoke(null, new object[] {this});
        }
        InteractableObject inter = GetComponent<InteractableObject>();
        if(inter != null && inter.AvailableOncePerCycle && !String.IsNullOrEmpty(inter.InteractableId)) {
            SaveFile.Instance.AddFlag(Utils.GetFormattedFlag(inter.InteractableId + "_Destroyed_[Cycle]"));
        }
        MonoBehaviour.Destroy(gameObject);
    }

    public void HandleObjectHit() {
        if(GetComponent<Rigidbody2D>() != null) {
            GetComponent<Rigidbody2D>().AddForce((transform.position - (transform.position - transform.up * 2)).normalized * 700, ForceMode2D.Force);
        }
    }
}
