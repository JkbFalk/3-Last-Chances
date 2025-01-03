public class NPCAbility_CrossbowShot : Ability {
    public static float Cooldown = 8;
    public NPCAbility_CrossbowShot(Unit ability_user) : base(ability_user) {
        DamageSources.Add(new DamageSource(200, 0, Constants.DamageType.Ranged));
        AddCustomSound("Release", "Bow/Bow_Release2", 0.4f);
        WaitTimeBeforeNextAction = 0.1f;
    }
}