using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttachObjectToBodyPart : MonoBehaviour
{
    public Unit Unit;
    public string BodyPartName;
    public bool AttachToBone;
    private bool _initialized = false;
    public bool WorldPositionStays = false;

    public void Initialize(Unit unit) {
        Unit = unit;
        Start();
    }

    void Start()
    {
        if(_initialized == false && Unit != null && string.IsNullOrEmpty(BodyPartName) == false && Unit.SpriteRenderers[BodyPartName] != null && (AttachToBone == false || Unit.SpriteRenderers[BodyPartName].Bone != null))
        {
            _initialized = true;
            transform.SetParent(AttachToBone ? Unit.SpriteRenderers[BodyPartName].Bone : Unit.SpriteRenderers[BodyPartName].SpriteRenderer.transform, true);
            if(!WorldPositionStays) {
                transform.position = transform.parent.position;
            }
        }
    }
}
