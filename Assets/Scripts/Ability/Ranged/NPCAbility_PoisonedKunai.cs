public class NPCAbility_PoisonedKunai : Ability {

    public static float Cooldown = 12;
    public NPCAbility_PoisonedKunai(Unit ability_user) : base(ability_user) {
        DamageSources.Add(new DamageSource(50, 0, Constants.DamageType.Ranged));
        AddCustomSound("Release", "Blade/Blade_Fly4", 0.4f);
        HitSoundType = Constants.HitSoundTypeEnum.SmallSharp;
        WaitTimeBeforeNextAction = 0.1f;
    }

    public override void ExtraBehaviourOnDamage(Damage damage)
    {
        damage.TargetOfDamage.AddEffect(new Effect_Poison(2, new(this)));
    }
}