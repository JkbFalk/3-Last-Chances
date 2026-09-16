using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class NPCAbility_FlameCage : Ability {

    public static float Cooldown = 20;
    public static AbilityFamily Family = AbilityFamily.Ignis;
    private AreaOfEffect _aoe;

    public NPCAbility_FlameCage(Unit ability_user) : base(ability_user)
    {
        AddCustomSound("Use", "Ability/Ability_Flamethrower", 0.2f);
        DamageSources.Add(new DamageSource(20, 5, Constants.DamageType.Magic));
        HitSoundType = Constants.HitSoundTypeEnum.Fire;
        HitSoundVolume = 0.2f;
    }

    public override void CallAbilityEvent1()
    {
        _aoe = Utils.CreateAreaOfEffect(new(this), "FlameCage");
        _aoe.GetComponentInParent<DestructibleEnvironment>().BaseHitPoints = 100 * CombatMath.GetExpectedPowerForLevel(User.Level);
        _aoe.GetComponentInParent<DestructibleEnvironment>().HitPoints = 100 * CombatMath.GetExpectedPowerForLevel(User.Level);
        _aoe.transform.parent.position = Target.transform.position;
        EventManager.OneSecondElapsedInGame.AddListener(ResetPotentialTargets);
        GameController.Instance.WaitAndRunMethod(20, new System.Action(() => { EventManager.OneSecondElapsedInGame.RemoveListener(ResetPotentialTargets); }));
    }

    public override void ExtraBehaviourOnDamage(DamageInstance damage)
    {
        damage.TargetOfDamage.AddEffect(new Effect_Burn(12 * User.MagicStagger.Current / 100, new(this)));
    }
}