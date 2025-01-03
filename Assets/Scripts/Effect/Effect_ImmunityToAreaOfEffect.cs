public class Effect_ImmunityToAreaOfEffect : Effect {

    public Effect_ImmunityToAreaOfEffect(SourceOfEffect source_of_effect) : base(source_of_effect) {
        Type = EffectType.Buff;
        Listeners.Add(EventManager.HitDealt);
    }

    public override void OnInvokeHitDealt(Damage damage)
    {
        if(damage.TargetOfDamage != TargetOfEffect) {
            return;
        }
        if(damage.DamagingObject is AreaOfEffect || damage.DamagingObject.GetType().IsSubclassOf(typeof(AreaOfEffect)))
        {
            damage.Injury = 0;
            damage.Stagger = 0;
            base.OnInvokeHitDealt(damage);
        }
    }
}