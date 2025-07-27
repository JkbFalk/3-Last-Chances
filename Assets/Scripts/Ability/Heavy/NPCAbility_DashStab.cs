public class NPCAbility_DashStab : Ability {
    public static float Cooldown = 6;
    public NPCAbility_DashStab(Unit ability_user) : base(ability_user)
    {
        DamageSources.Add(new DamageSource(70, 250, Constants.DamageType.Heavy) { KnockbackInMeters = 2f });
        WaitTimeBeforeNextAction = 0.2f;
        HitSoundVolume = 0.6f;
        Properties.Add(Property.Unstoppable);
        /*Properties.Add(AbilityProperty.CounteredByBackstep);
        Properties.Add(AbilityProperty.CounteredByRoll);
        Properties.Add(AbilityProperty.CounteredByRiposte);*/
    }

    public override void CallAbilityEvent1() {
        ChaseCurrentTargetAtGivenDegreeAngle(4, 25);
        User.Actions.DisplayExtremeDangerSign();
    }

    public override void ExtraBehaviourOnDamage(Damage damage)
    {
        //damage.TargetOfDamage.AddEffect(new Effect_Slow(50, this));
    }
}