using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class NPCAbility_FlameCage : Ability {

    public static float Cooldown = 20;
    public static AbilityFamily Family = AbilityFamily.Ignis;
    private AreaOfEffect _aoe;

    public NPCAbility_FlameCage(Unit ability_user) : base(ability_user) {
        AddCustomSound("Use", "Ability/Ability_Flamethrower", 0.2f);
        DamageSources.Add(new DamageSource(20, 5, Constants.DamageType.Magic));
        HitSoundType = Constants.HitSoundTypeEnum.Fire;
        HitSoundVolume = 0.2f;
    }

    public override void ActionToPerformAfterIntervals() {
        if(_aoe != null && _aoe.gameObject != null && _aoe.gameObject.IsDestroyed() == false) {
            ResetPotentialTargets();
        }
    }

    public override void CallAbilityEvent1()
    {
        _aoe = Utils.CreateAreaOfEffect(new(this), "FlameCage");
        _aoe.GetComponentInParent<DestructibleEnvironment>().BaseHitPoints = 100 * Utils.GetExpectedPowerForLevel(User.Level);
        _aoe.GetComponentInParent<DestructibleEnvironment>().HitPoints = 100 * Utils.GetExpectedPowerForLevel(User.Level);
        _aoe.transform.parent.position = Target.transform.position;
        PerformActionAfterIntervals(80, 0.25f);
    }

    public override void ExtraBehaviourOnDamage(Damage damage)
    {
        damage.TargetOfDamage.AddEffect(new Effect_Burn(3 * User.MagicStagger.Current / 100, new(this)));
    }
}