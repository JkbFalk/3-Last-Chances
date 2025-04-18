using System.Collections.Generic;
using UnityEngine;

public class NPCAbility_BlockAndSlash : Ability {

    public static float Cooldown = 6;

    private Effect_ChangeStat _blockEffect;
    public NPCAbility_BlockAndSlash(Unit ability_user) : base(ability_user) {
        DamageSources.Add(new DamageSource(70, 400, Constants.DamageType.Light));
        AddCustomSound("Slash", "Generic/Generic_Swoosh4", 0.8f);
        HitSoundType = Constants.HitSoundTypeEnum.LongSharp;
        WaitTimeBeforeNextAction = 0.1f;
        EventManager.HitDealt.AddListener(CheckHit);
    }

    public override void OnAbilityEnd()
    {
        base.OnAbilityEnd();
        EventManager.HitDealt.RemoveListener(CheckHit);
    }

    public override void CallAbilityEvent1()
    {
        _blockEffect = new Effect_ChangeStat(Player.Instance.DamageReduction, new(this)) {PercentageModifier = 1000};
        User.AddEffect(_blockEffect);
    }

    public override void CallAbilityEvent2()
    {
        _blockEffect.EndThisEffect();
        EndThisAbility();
    }

    public void CheckHit(Damage damage) {
        if(damage.TargetOfDamage == User && _blockEffect != null && _blockEffect.EffectEnded == false) {
            _blockEffect.EndThisEffect();
            Utils.PlaySoundEffect(User.AudioSource, "Steel/SteelBlock7", 0.8f);
            Utils.PlaySoundEffect(User.AudioSource, "Blade/Blade_Swing5", 0.8f);
            User.PlayAnimation("BlockAndSlash", 0, 0.6f);
        }
    }
}