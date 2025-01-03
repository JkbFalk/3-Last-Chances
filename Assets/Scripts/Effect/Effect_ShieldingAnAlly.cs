public class Effect_ShieldingAnAlly : Effect {

    public Effect_ShieldingAnAlly(SourceOfEffect source_of_effect) : base(source_of_effect) {
        Listeners.Add(EventManager.HitDealt);
        ScaleWithControlAndTenacity = false;
    }

    public override void OnInvokeHitDealt(Damage damage) {
        if(damage.TargetOfDamage != TargetOfEffect) {
            return;
        }
        if(SourceOfEffect.Target.Actions.enabled)
        {
            damage.Stagger = (damage.Stagger / 2) + damage.Injury / 10;
            damage.Injury = 0;
        }
        else
        {
            EndThisEffect();
        }
        base.OnInvokeHitDealt(damage);
    }
}