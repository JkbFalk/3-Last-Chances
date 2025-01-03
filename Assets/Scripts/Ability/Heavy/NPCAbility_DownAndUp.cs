using System.Collections.Generic;
using UnityEngine;

public class NPCAbility_DownAndUp : Ability {
    public static float Cooldown = 7;

    public NPCAbility_DownAndUp(Unit ability_user) : base(ability_user) {
        WaitTimeBeforeNextAction = 0.5f;
        DamageSources.Add(new DamageSource(50, 250, Constants.DamageType.Heavy));
        DamageSources.Add(new DamageSource(150, 100, Constants.DamageType.Heavy, "2"));
        AbilityModifiers.Add(Constants.AbilityModifier.CounteredByRiposte);
    }

    public override void CallAbilityEvent1() {
        AbilityModifiers.Clear();
        AbilityModifiers.Add(Constants.AbilityModifier.CounteredByBackstep);
    }
}