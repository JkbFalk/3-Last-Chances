using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class NPCAbility_FlameWall : Ability {

    public static float Cooldown = 25;
    public static AbilityFamily Family = AbilityFamily.Ignis;
    private AreaOfEffect _aoe;

    public NPCAbility_FlameWall(Unit ability_user) : base(ability_user) {
        AddCustomSound("Use", "Ability/Ability_Flamethrower", 0.2f);
        DamageSources.Add(new DamageSource(20, 25, Constants.DamageType.Magic));
        HitSoundType = Constants.HitSoundTypeEnum.Fire;
        HitSoundVolume = 0.2f;
        NameOfAnimationToAutoPlay = "FlameWallNPC";
    }

    public override void ActionToPerformAfterIntervals() {
        if(_aoe != null && _aoe.gameObject != null && _aoe.gameObject.IsDestroyed() == false) {
            ResetPotentialTargets();
        }
    }

    public override void CallAbilityEvent1()
    {
        _aoe = Utils.CreateAreaOfEffect(new(this), "FlameWallNPC");
        PerformActionAfterIntervals(50, 0.25f);
    }

    public override void ExtraBehaviourOnDamage(Damage damage)
    {
        damage.TargetOfDamage.AddEffect(new Effect_Burn(10 * User.MagicStagger.Current / 100, new(this)));
    }
}