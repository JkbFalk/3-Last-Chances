public class Effect_WindRushUltimateProtection : Effect
{
    private readonly Unit _allowedDamageSource;

    public Effect_WindRushUltimateProtection(SourceOfEffect source_of_effect, Unit allowed_damage_source) : base(source_of_effect)
    {
        _allowedDamageSource = allowed_damage_source;
        Id = "WindRush_UltimateProtection";
        IsRemovable = false;
        Listeners.Add(EventManager.AfterHitDamageCalculation);
    }

    public bool AllowsEffectsFrom(Unit source)
    {
        return source == _allowedDamageSource;
    }

    public override void OnInvokeAfterHitDamageCalculation(DamageInstance damage)
    {
        if (damage.TargetOfDamage == TargetOfEffect && damage.SourceOfDamage?.User != _allowedDamageSource)
        {
            damage.Injury = 0;
            damage.Stagger = 0;
            base.OnInvokeAfterHitDamageCalculation(damage);
        }
    }
}