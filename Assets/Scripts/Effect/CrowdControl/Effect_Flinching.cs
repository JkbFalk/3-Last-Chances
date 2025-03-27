using UnityEngine;

public class Effect_Flinching : Effect_HardCrowdControl
{

    public Effect_Flinching(SourceOfEffect source_of_effect) : base(source_of_effect)
    {
        BehaviourWhenDuplicateEffect = BehaviourWhenDuplicateEffectEnum.AddDuration;
        PathToEffectGraphic = "Effect/Stun";
    }

    public override void AdditionalActionsOnSettingTargetOfEffect(Unit target_of_effect)
    {
        if(target_of_effect is Player && (Player.Instance.CurrentStance.StanceEffectType == typeof(Stance_PowerWithoutLimit) || Player.Instance.CurrentStance.StanceEffectType == typeof(Stance_MindOverMatter))) {
            NameOfAnimationToAutoPlay = "Flinching";
        }
        else {
            NameOfAnimationToAutoPlay = "Flinching" + (TargetOfEffect.CurrentWeaponDamageCategory == Constants.DamageType.Magic || TargetOfEffect.CurrentWeaponDamageCategory == Constants.DamageType.None ? "" : "_" + TargetOfEffect.CurrentWeaponCategory.ToString());
        }
    }

    public override void OnEnd()
    {
        base.OnEnd();
        if(TargetOfEffect.UnitAI != null)
        {
            TargetOfEffect.UnitAI.WaitTimeBeforeNextAction = (int)(15 / TargetOfEffect.UnitAI.Aggressiveness);        
        }
    }
}