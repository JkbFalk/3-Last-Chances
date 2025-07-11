public class Effect_ShieldedByAlly : Effect {

    public Effect_ShieldedByAlly(SourceOfEffect source_of_effect) : base(source_of_effect) {
        ScaleWithControlAndTenacity = false;
        Type = EffectType.Buff;
        Listeners.Add(EventManager.AfterHitDamageCalculation);
    }

    public override void OnStart() {
        base.OnStart();
        AddVisualEffectOnTarget("VisualEffect_ShieldedByAlly");
    }

    public override void OnInvokeAfterHitDamageCalculation(Damage damage) {
        if(damage.TargetOfDamage != TargetOfEffect) {
            return;
        }
        if(!SourceOfEffect.User.KnockedOut)
        {
        Damage damage2 = new Damage(UnitCreatingTheEffect, damage.SourceOfDamage, damage.DamagingObject);
        damage2.Injury = damage.Injury;
        damage2.Stagger = damage.Stagger;
        damage2.CalculateAndApplyDamage();
        damage.Injury = 0;
        damage.Stagger = 0;
        }
        else
        {
            EndThisEffect();
        }
        base.OnInvokeAfterHitDamageCalculation(damage);
    }
}