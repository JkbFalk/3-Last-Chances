public class NPCAbility_RangedItemStab : Ability {
    public static float Cooldown = 6;
    public NPCAbility_RangedItemStab(Unit ability_user) : base(ability_user) {
        DamageSources.Add(new DamageSource(100, 150, Constants.DamageType.Ranged));
        AddCustomSound("Swing", "Blade/Blade_Swing3", 0.4f);
        WaitTimeBeforeNextAction = 0.3f;
        HitSoundType = Constants.HitSoundTypeEnum.SmallSharp;
    }
}