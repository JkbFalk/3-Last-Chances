using System.Collections.Generic;
using UnityEngine;

public class NPCAbility_GreatswordWindmill : Ability {
    public static float Cooldown = 6;
    public NPCAbility_GreatswordWindmill(Unit ability_user) : base(ability_user) {
        WaitTimeBeforeNextAction = 0.2f;
        DamageSources.Add(new DamageSource(150, 600, Constants.DamageType.Heavy));
        AddCustomSound("Swing1", "Greatsword/Greatsword_Swing23", 0.6f);
        AddCustomSound("Swing2", "Greatsword/Greatsword_Swing22", 0.6f);
        AddCustomSound("Explosion", "Explosion/Ground Explosion", 0.2f);
        AbilityModifiers.Add(Constants.AbilityModifier.CounteredByRiposte);
    }

    public override void ExtraBehaviourOnDamage(Damage damage)
    {
        damage.TargetOfDamage.AddEffect(new Effect_KnockedBack(damage, new(this)));
    }
}