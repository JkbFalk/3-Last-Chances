using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SortingOrder : MonoBehaviour {
    public int AdjustSortingOrder = 0;
    private SpriteRenderer _spriteRenderer;
    private ParticleSystemRenderer _particleSystemRenderer;
    private int _colliderAdjustment = 0;
    private Vector2 _previousPosition;
    [HideInInspector]
    public bool ShouldAdjust = true;
    public bool LowerChildrenInFront = true;
    private Dictionary<GameObject, int> _childrenAdjusts = new();

    private void Start() {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _particleSystemRenderer = GetComponent<ParticleSystemRenderer>();
        BoxCollider2D coll = GetComponent<BoxCollider2D>();
        if(coll != null) {
            _colliderAdjustment = coll != null ? (int)((coll.offset.y + coll.size.y / 2) * 100 * transform.lossyScale.y) : 0;
        }
        foreach(SortingOrder so in GetComponentsInChildren<SortingOrder>()) {
            if(so.gameObject != gameObject) {
                so.ShouldAdjust = false;
                _childrenAdjusts.Add(so.gameObject, so.AdjustSortingOrder);
            }
        }
        Rigidbody2D body = GetComponentInParent<Rigidbody2D>();
        if(body != null && body.bodyType != RigidbodyType2D.Static) {
            GameController.Instance.DynamicSortingOrders.Add(this);
        }
        else {
            UpdateSortingOrder();
        }
    }

    public void OnDestroy() {
        if(GameController.Instance.DynamicSortingOrders.Contains(this)) {
            GameController.Instance.DynamicSortingOrders.Remove(this);
        }
    }

    public void UpdateSortingOrder() {
        if(ShouldAdjust == false) {
            return;
        }
        if(_previousPosition == null) {
            _previousPosition = transform.position;
        }
        if((int)(_previousPosition.y * 10) == (int)(transform.position.y * 10)) {
            return;
        }
        float height = transform.position.y;
        int SRSortingOrder = 0;
        if (_spriteRenderer != null) {
            _spriteRenderer.sortingOrder = (-1) * Mathf.RoundToInt(height * 100) - _colliderAdjustment + AdjustSortingOrder;
            SRSortingOrder = _spriteRenderer.sortingOrder;
        }
        else {
            SRSortingOrder = (-1) * Mathf.RoundToInt(height * 100) - _colliderAdjustment + AdjustSortingOrder;
        }
        if (_particleSystemRenderer != null)
        {
            _particleSystemRenderer.sortingOrder = (-1) * Mathf.RoundToInt(height * 100) - _colliderAdjustment + AdjustSortingOrder;
        }
        List<SpriteRenderer> childrenSRs;
        if(LowerChildrenInFront) {
            childrenSRs = GetComponentsInChildren<SpriteRenderer>().Where(sr => sr.gameObject != gameObject && sr.sortingLayerName != "UI").OrderByDescending(sr => sr.transform.position.y).ToList();
        }
        else {
            childrenSRs = GetComponentsInChildren<SpriteRenderer>().Where(sr => sr.gameObject != gameObject && sr.sortingLayerName != "UI").OrderBy(sr => sr.transform.position.y).ToList();
        }
        for(int i = 0; i < childrenSRs.Count; i++) {
            int extraAdjust = _childrenAdjusts.ContainsKey(childrenSRs[i].gameObject) ? _childrenAdjusts[childrenSRs[i].gameObject] : 0;
            childrenSRs[i].sortingOrder = SRSortingOrder + i + 1 + extraAdjust;
        }
        _previousPosition = transform.position;
    }
}