using System.Collections.Generic;
using UnityEngine;

public class NPCAbility_TripleHeavySlash : Ability {
    public static float Cooldown = 6;

    private int _stage = 0;
    public NPCAbility_TripleHeavySlash(Unit ability_user) : base(ability_user) {
        WaitTimeBeforeNextAction = 0.2f;
        DamageSources.Add(new DamageSource(20, 200, Constants.DamageType.Heavy));
        DamageSources.Add(new DamageSource(80, 200, Constants.DamageType.Heavy, "2"));
        DamageSources.Add(new DamageSource(200, 200, Constants.DamageType.Heavy, "3"));
        AddCustomSound("Swing1", "Greatsword/Greatsword_Swing15", 0.5f);
        AddCustomSound("Swing2", "Greatsword/Greatsword_Swing18", 0.6f);
        AddCustomSound("Swing3", "Greatsword/Greatsword_Swing16", 0.7f);
    }

    public override void CallAbilityEvent1()
    {
        ChaseCurrentTargetAtGivenDegreeAngle(_stage == 0 ? 2 : _stage == 1 ? 1.4f : 0.7f, 45);
    }

    public override void ExtraBehaviourOnDamage(DamageInstance damage)
    {
        if(WeaponCollisionName == "Default") {
            User.PlayAnimation("TripleHeavySlash", 0.05f, 0.26f);
        }
        else if(WeaponCollisionName == "2") {
            User.PlayAnimation("TripleHeavySlash", 0.05f, 0.58f);
        }
    }
}