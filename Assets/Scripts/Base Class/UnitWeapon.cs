using UnityEngine;

public class UnitWeapon : DamagingObject {
    private SpriteRenderer _parentSpriteRenderer;
    public bool UpdateChildrenSortingOrder = false;
    private Renderer[] _childRenderers;
    public GameObject WeaponTrail;
    public BoxCollider2D WeaponHitbox;

    private void Start() {
        Owner = GetComponentInParent<Unit>();
        WeaponHitbox = GetComponent<BoxCollider2D>();
        if (transform.parent != null) {
            _parentSpriteRenderer = transform.parent.GetComponent<SpriteRenderer>();
        }
        if (UpdateChildrenSortingOrder) {
            _childRenderers = GetComponentsInChildren<Renderer>();
        }
        if (transform.Find("Weapon Trail") != null && (name == "Heavy Bone" || name == "Light Right Bone" || name == "Light Left Bone"))
        {
            WeaponTrail = transform.Find("Weapon Trail").gameObject;
        }
    }

    public void LateUpdate() {
        if (UpdateChildrenSortingOrder && _parentSpriteRenderer != null) {
            foreach (Renderer renderer in _childRenderers) {
                renderer.sortingOrder = _parentSpriteRenderer.sortingOrder;
            }
        }
    }

    public void ChangeWeaponDealingDamage(bool deals_damage = false, Ability source_of_damage = null)
    {
        DealingDamage = deals_damage;
        SourceAbility = source_of_damage;
        if (WeaponTrail == null)
        {
            return;
        }
        if (deals_damage)
        {
            WeaponTrail.GetComponent<ParticleSystem>().Play();
        } 
        else
        {
            WeaponTrail.GetComponent<ParticleSystem>().Stop();
        }
    }
}