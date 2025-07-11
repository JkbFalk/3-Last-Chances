using System.Collections.Generic;
using UnityEngine;

public class NPCAbility_BoulderPunch : Ability {

    public static float Cooldown = 8;
    public static AbilityFamily Family = AbilityFamily.Molis;
    private GameObject boulderVFX;

    public NPCAbility_BoulderPunch(Unit ability_user) : base(ability_user) {
        DamageSources.Add(new DamageSource(30, 300, Constants.DamageType.Magic) {KnockbackInMeters = 0.7f, CustomHitSound = "Earth/Earth_Crack6"});
        AddCustomSound("Crack", "Earth/Earth_Crack5", 0.6f);
        AddCustomSound("Hit", "Earth/Earth_Punch2", 0.6f);
        HitSoundType = Constants.HitSoundTypeEnum.LargeBlunt;
        WaitTimeBeforeNextAction = 0.1f;
        Properties.Add(Property.ImmuneToFlinch);
        EffectsAffectingUserDuringAbility = new List<Effect>() { new Effect_Immovable(new(this)), new Effect_RootedInPlace(new(this)) };
    }

    public override void CallAbilityEvent1()
    {
        Utils.CreateVisualEffect(new(this), "EarthCrack");
        boulderVFX = Utils.CreateVisualEffect(new(this), "BoulderPunch");
    }

    public override void CallAbilityEvent2()
    {
        if(UnityEngine.Random.Range(0, 100) < 50) {
            User.PlayAnimation("BoulderPunch", 0, 0.47f);
            MonoBehaviour.Destroy(boulderVFX);
        }
    }

    public override void CallAbilityEvent3()
    {
        if(boulderVFX != null) {
            MonoBehaviour.Destroy(boulderVFX);
        }
    }
}