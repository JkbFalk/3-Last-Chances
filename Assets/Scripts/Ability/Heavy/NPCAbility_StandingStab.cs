public class NPCAbility_StandingStab : Ability {
    public static float Cooldown = 3;
    public NPCAbility_StandingStab(Unit ability_user) : base(ability_user) {
        DamageSources.Add(new DamageSource(150, 1000, Constants.DamageType.Heavy));
        WaitTimeBeforeNextAction = 0.1f;
        HitSoundVolume = 0.6f;
        Properties.Add(Property.CounteredByBackstep);
        Properties.Add(Property.CounteredByRoll);
        Properties.Add(Property.CounteredByRiposte);
    }
    public override void ExtraBehaviourOnDamage(Damage damage)
    {
        //damage.TargetOfDamage.AddEffect(new Effect_Stun(this), 2);
    }
}