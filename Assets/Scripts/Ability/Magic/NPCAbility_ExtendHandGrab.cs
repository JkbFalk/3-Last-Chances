using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class NPCAbility_ExtendHandGrab : Ability {
    public static float Cooldown = 25;

    private Unit _enemyHit;

    public NPCAbility_ExtendHandGrab(Unit ability_user) : base(ability_user) {
        AddCustomSound("Swing", "Steel/SteelExtend", 0.5f);
        DamageSources.Add(new DamageSource(50, 350, Constants.DamageType.Magic));
        WaitTimeBeforeNextAction = 1f;
    }

    public override void ExtraBehaviourOnDamage(Damage damage)
    {
        damage.TargetOfDamage.AddEffect(new Effect_KnockedBack(damage, new(this)));
        damage.TargetOfDamage.AddEffect(new Effect_Onslaught(100, new(this)), 5);
        _enemyHit = damage.TargetOfDamage;
        User.UnitAI.PerformAnAttack();
    }

    public override void CallAbilityEvent1() {
        if(_enemyHit != null) {
            _enemyHit.PushIntoPosition(User.transform.position - new Vector3(0.5f * (User.Actions.IsFlipped ? -1 : 1), 0), this);
        }
    }
}