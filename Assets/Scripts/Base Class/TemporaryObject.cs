using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class TemporaryObject : WorldObject {
    [HideInInspector]
    public string SourceOfHitName;
    public Ability SourceAbility { get; set; }
    public float DestroyChildrenNSecondsAfterDisappearing = 0;

    public float DeactivateNSecondsAfterStart = 0;
    public float DeactivateTimer = 0;
    private ParticleSystem[] _cachedParticleSystems;
    private SetChildActiveAfterNSeconds[] _cachedTimedChildren;

    private float _baseDuration = 0;

    public float BaseDuration {
        get => _baseDuration;
        set {
            _baseDuration = value;
            RemainingDuration = (int)(50 * _baseDuration);
        }
    }

    public bool AlwaysSameSimulationSpeed = false;

    public int RemainingDuration { get; set; } = 0;
    public bool IsDisappearing = false;
    public float DisappearTimeInSeconds = 0.25f;
    [HideInInspector]
    public SpriteRenderer WeaponSpriteRenderer;
    public bool IsDestroyed = false;
    [HideInInspector]
    public List<ParticleSystemRenderer> ParticleSystemRenderers;

    private void Awake() {
        WeaponSpriteRenderer = GetComponent<SpriteRenderer>();
        ParticleSystemRenderers = GetComponentsInChildren<ParticleSystemRenderer>(true).ToList();
        if(DeactivateNSecondsAfterStart != 0)
        {
            DeactivateTimer = DeactivateNSecondsAfterStart * 50;
        }
        _cachedParticleSystems = GetComponentsInChildren<ParticleSystem>(true);
        _cachedTimedChildren = GetComponentsInChildren<SetChildActiveAfterNSeconds>(true);
    }

    public virtual void AdditionalActionsAfterUpdate() {
    }

    public void MakeObjectDisappear(float disappear_time = 0.25f)
    {
        if(this is DamagingObject) {
            GetComponent<DamagingObject>().DealingDamage = false;
        }
        IsDisappearing = true;
        DisappearTimeInSeconds = disappear_time;
        if(DisappearTimeInSeconds == 0) {
            OnDestroyObject();
        }
    }

    public void CleanUpAfter(float seconds = 5) {
        GameController.Instance.WaitAndRunMethod(seconds, CleanUpObject);
    }

    public void CleanUpObject() {
        if(IsDestroyed == false && gameObject != null && gameObject!= null && gameObject.GetComponent<TemporaryObject>() != null) {
            gameObject.GetComponent<TemporaryObject>().MakeObjectDisappear();
        }
    }

    public void FixedUpdate() {
        if(DeactivateTimer != 0)
        {
            DeactivateTimer--;
            if(DeactivateTimer <= 0)
            {
                enabled = false;
                if(this is DamagingObject)
                {
                    DamagingObject thisObj = (DamagingObject)this;
                    thisObj.DealingDamage = false;
                }
            }
        }
        if (BaseDuration > 0 && RemainingDuration > 0) {
            RemainingDuration--;
            if (RemainingDuration == 0) {
                IsDisappearing = true;
            }
        }
        if (IsDisappearing) {
            if (WeaponSpriteRenderer != null) {
                if(WeaponSpriteRenderer.color.a == 1) {
                    WeaponSpriteRenderer.color = new Color(WeaponSpriteRenderer.color.r, WeaponSpriteRenderer.color.g, WeaponSpriteRenderer.color.b, 0.5f);
                }
                else if (WeaponSpriteRenderer.color.a > 0) {
                    WeaponSpriteRenderer.color = new Color(WeaponSpriteRenderer.color.r, WeaponSpriteRenderer.color.g, WeaponSpriteRenderer.color.b, WeaponSpriteRenderer.color.a - 1f / (DisappearTimeInSeconds * 50));
                }
                else {
                    OnDestroyObject();
                }
            }
            else if (ParticleSystemRenderers != null && ParticleSystemRenderers.Count > 0) {
                foreach(ParticleSystemRenderer psr in ParticleSystemRenderers) {
                    if (psr != null && psr.material.HasFloat("_Alpha") && psr.material.GetFloat("_Alpha") == 1)
                    {
                        psr.material.SetFloat("_Alpha", 0.5f);
                    }
                    else if (psr != null && psr.material.HasFloat("_Alpha") && psr.material.GetFloat("_Alpha") > 0)
                    {
                        psr.material.SetFloat("_Alpha", psr.material.GetFloat("_Alpha") - 1f / (DisappearTimeInSeconds * 0.65f * 50));
                    }
                    if (psr != null && psr.material.color.a == 1) {
                        psr.material.color = new Color(psr.material.color.r, psr.material.color.g, psr.material.color.b, 0.5f);
                    }
                    else if (psr != null && psr.material.color.a > 0) {
                        psr.material.color = new Color(psr.material.color.r, psr.material.color.g, psr.material.color.b, psr.material.color.a - 1f / (DisappearTimeInSeconds * 0.65f * 50));
                    }
                }
                if((ParticleSystemRenderers[0].material.HasFloat("_Alpha") && ParticleSystemRenderers[0].material.GetFloat("_Alpha") <= 0) || ParticleSystemRenderers[0].material.color.a <= 0) {
                    OnDestroyObject();
                }
            }

        }
        AdditionalActionsAfterFixedUpdate();
    }

    public void ChangeAlphaOfAllParticleSystems(float alpha) {

    }

    public virtual void AdditionalActionsAfterFixedUpdate() {
    }

    public void OnDestroy() {
        IsDestroyed = true;
    }

    protected virtual void OnDestroyObject() {
        if(IsDestroyed) {
            return;
        }
        if (DestroyChildrenNSecondsAfterDisappearing > 0)
        {
            for (int i = transform.childCount - 1; i >= 0; i--)
            {
                GameObject child = transform.GetChild(i).gameObject;
                child.SetActive(true);
                child.AddComponent(typeof(DestroyGameObjectAfterGivenTime));
                child.GetComponent<DestroyGameObjectAfterGivenTime>().DestroyAfterSeconds = DestroyChildrenNSecondsAfterDisappearing;
                child.transform.SetParent(transform.parent);
                child.transform.position = gameObject.transform.position;
            }
        }
        foreach (DestroyGameObjectAfterGivenTime item in GetComponentsInChildren<DestroyGameObjectAfterGivenTime>())
        {
            if (item.DetachIfParentDestroyed)
            {
                item.transform.SetParent(transform.parent, true);
            }
    }
        IsDestroyed = true;
        ResetVisualsForPool();
        if (GetComponent<PooledObject>() != null) {
            ObjectPool.Release(gameObject);
            return;
        }
        MonoBehaviour.Destroy(gameObject);
    }

    public void ResetForPoolReuse() {
        IsDestroyed = false;
        IsDisappearing = false;
        RemainingDuration = BaseDuration > 0 ? (int)(50 * BaseDuration) : 0;
        enabled = true;
        if (DeactivateNSecondsAfterStart != 0) {
            DeactivateTimer = DeactivateNSecondsAfterStart * 50;
        }
        if (this is DamagingObject damagingObject) {
            damagingObject.DealingDamage = true;
            damagingObject.WasStopped = false;
        }
        ResetVisualsForPool();
        foreach (ParticleSystem system in GetComponentsInChildren<ParticleSystem>(true)) {
            system.Clear(true);
            system.Play(true);
        }
        foreach (SetChildActiveAfterNSeconds timedChild in GetComponentsInChildren<SetChildActiveAfterNSeconds>(true)) {
            timedChild.ResetForReuse();
        }
        if (_cachedParticleSystems != null)
        {
            foreach (ParticleSystem system in _cachedParticleSystems) 
            {
                system.Clear(true);
                system.Play(true);
            }
        }

        if (_cachedTimedChildren != null)
        {
            foreach (SetChildActiveAfterNSeconds timedChild in _cachedTimedChildren) 
            {
                timedChild.ResetForReuse();
            }
        }
    }

    private void ResetVisualsForPool() {
        if (WeaponSpriteRenderer != null) {
            Color color = WeaponSpriteRenderer.color;
            WeaponSpriteRenderer.color = new Color(color.r, color.g, color.b, 1f);
        }
        if (ParticleSystemRenderers == null) {
            return;
        }
        foreach (ParticleSystemRenderer psr in ParticleSystemRenderers) {
            if (psr == null) {
                continue;
            }
            if (psr.material.HasFloat("_Alpha")) {
                psr.material.SetFloat("_Alpha", 1f);
            }
            Color color = psr.material.color;
            psr.material.color = new Color(color.r, color.g, color.b, 1f);
        }
    }
}