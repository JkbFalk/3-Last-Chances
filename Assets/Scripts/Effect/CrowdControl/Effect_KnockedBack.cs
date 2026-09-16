using UnityEngine;

public class Effect_KnockedBack : Effect_HardCrowdControl
{
    private int _stage = -1;
    private DamageInstance _damage;

    public Effect_KnockedBack(DamageInstance damage, SourceOfEffect source_of_effect) : base(source_of_effect)
    {
        PriorityLevel = 6;
        _damage = damage;
        BehaviourWhenDuplicateEffect = BehaviourWhenDuplicateEffectEnum.ExtendDuration;
    }

    public override void OnStart() {
        base.OnStart();
        if(_damage.DamagingObject != null && _damage.DamagingObject.transform.position.x > TargetOfEffect.transform.position.x && TargetOfEffect.Actions.IsFlipped) {
            TargetOfEffect.Actions.IsFlipped = false;
        }
        else if(_damage.DamagingObject != null && _damage.DamagingObject.transform.position.x < TargetOfEffect.transform.position.x && !TargetOfEffect.Actions.IsFlipped) {
            TargetOfEffect.Actions.IsFlipped = true;
        }
        else if(_damage.SourceOfDamage.User.transform.position.x > TargetOfEffect.transform.position.x && TargetOfEffect.Actions.IsFlipped) {
            TargetOfEffect.Actions.IsFlipped = false;
        }
        else if(_damage.SourceOfDamage.User.transform.position.x < TargetOfEffect.transform.position.x && !TargetOfEffect.Actions.IsFlipped) {
            TargetOfEffect.Actions.IsFlipped = true;
        }
        GameController.Instance.WaitAndRunMethod(0.02f, UnlockKnockedBackStages);
        GameController.Instance.WaitAndRunMethod(3f * CombatMath.GetEffectiveCrowdControlDuration(_damage.SourceOfDamage.User, TargetOfEffect), EndKnockedBack);
    }

    public void UnlockKnockedBackStages() {
        _stage = 0;
    }

    public override void OnFixedUpdate() {
        if(_stage == 0 && TargetOfEffect.Rigidbody2D.linearVelocity.magnitude < 1) {
            _stage++;
            TargetOfEffect.PlayAnimation("KnockedBack2", 0.05f);
        }
        else if(_stage == 1 && TargetOfEffect.Rigidbody2D.linearVelocity.magnitude < 0.1f) {
            _stage++;
            TargetOfEffect.PlayAnimation("KnockedBack3", 0.05f);
            GameController.Instance.WaitAndRunMethod(0.5f / TargetOfEffect.Tenacity.Current / 100, EndKnockedBack);
        }
    }

    public void EndKnockedBack() {
        if(!EffectEnded) {
            UnitBoundsManager.Instance?.ValidateAndEnforceBounds(TargetOfEffect);
            EndThisEffect();
        }
    }
}