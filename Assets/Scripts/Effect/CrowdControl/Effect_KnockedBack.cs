using UnityEngine;

public class Effect_KnockedBack : Effect_HardCrowdControl
{
    private int _stage = -1;
    private Damage _damage;

    public Effect_KnockedBack(Damage damage, SourceOfEffect source_of_effect) : base(source_of_effect)
    {
        PriorityLevel = 6;
        _damage = damage;
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
        Debug.Log($"CALC: tenacity: {Utils.GetEffectiveCrowdControlDuration(_damage.SourceOfDamage.User, TargetOfEffect)}, result: {3f * Utils.GetEffectiveCrowdControlDuration(_damage.SourceOfDamage.User, TargetOfEffect)}");
        GameController.Instance.WaitAndRunMethod(3f * Utils.GetEffectiveCrowdControlDuration(_damage.SourceOfDamage.User, TargetOfEffect), EndKnockedBack);
    }

    public void UnlockKnockedBackStages() {
        Debug.Log("TRANSITION TO STAGE 1");
        _stage = 0;
    }

    public override void OnFixedUpdate() {
        if(_stage == 0 && TargetOfEffect.Rigidbody2D.velocity.magnitude < 1) {
            _stage++;
            Debug.Log("TRANSITION TO STAGE 2");
            TargetOfEffect.PlayAnimation("KnockedBack2", 0.05f);
        }
        else if(_stage == 1 && TargetOfEffect.Rigidbody2D.velocity.magnitude < 0.1f) {
            _stage++;
            Debug.Log("TRANSITION TO STAGE 3");
            TargetOfEffect.PlayAnimation("KnockedBack3", 0.05f);
            GameController.Instance.WaitAndRunMethod(0.5f / TargetOfEffect.Tenacity.Current / 100, EndKnockedBack);
        }
    }

    public void EndKnockedBack() {
        if(!EffectEnded) {
            Debug.Log("ending knocked back");
            EndThisEffect();
        }
    }
}