using System.Collections.Generic;
using UnityEngine;

public class NPCAbility_FastJabs : Ability {

    public static float Cooldown = 4;


    public NPCAbility_FastJabs(Unit ability_user) : base(ability_user) {
        DamageSources.Add(new DamageSource(75, 30, Constants.DamageType.Light, "SmallCircleAoE"));
        DamageSources.Add(new DamageSource(100, 250, Constants.DamageType.Light, "3"));
        AddCustomSound("Swing1", "Generic/Generic_Swoosh2", 0.6f);
        AddCustomSound("Swing2", "Generic/Generic_Swoosh3", 0.6f);
        HitSoundType = Constants.HitSoundTypeEnum.SmallBlunt;
        WaitTimeBeforeNextAction = 0.1f;
        DamageTriggerLimit = DamageTriggerLimitType.OncePerUnitFromEachSource;
    }

    public override void CallAbilityEvent1()
    {
        AreaOfEffect aoe = Utils.CreateAreaOfEffect(new(this), "SmallCircleAoE");
        aoe.gameObject.name = "3";
    }
}