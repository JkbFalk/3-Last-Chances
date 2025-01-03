public class NPCAbility_StandingStab : Ability {
    public static float Cooldown = 3;
    public NPCAbility_StandingStab(Unit ability_user) : base(ability_user) {
        DamageSources.Add(new DamageSource(150, 100, Constants.DamageType.Heavy));
        WaitTimeBeforeNextAction = 0.1f;
        HitSoundVolume = 0.6f;
        /*AbilityModifiers.Add(Constants.AbilityModifier.CounteredByBackstep);
        AbilityModifiers.Add(Constants.AbilityModifier.CounteredByRoll);
        AbilityModifiers.Add(Constants.AbilityModifier.CounteredByRiposte);*/
    }
    public override void ExtraBehaviourOnDamage(Damage damage)
    {
        //damage.TargetOfDamage.AddEffect(new Effect_Stun(this), 2);
    }
}