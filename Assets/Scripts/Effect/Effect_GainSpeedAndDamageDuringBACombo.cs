using System;
using UnityEngine;

public class Effect_GainSpeedAndDamageDuringBACombo : Effect {
    //Value = 10 dmg or stagger / 100p

    public Effect_ChangeStat HeavySpeed;
    public Effect_ChangeStat HeavyInjury;
    public Effect_ChangeStat HeavyStagger;
    public float SpeedBonusPerHit;
    public float DamageBonusPerHit;

    public Effect_GainSpeedAndDamageDuringBACombo(SourceOfEffect source_of_effect) : base(source_of_effect) {
        Type = EffectType.Buff;
        TriggersOncePerAbility = true;
        Listeners.Add(EventManager.AbilityUsed);
    }

    public override void OnEffectValueChanged()
    {
        SpeedBonusPerHit *= NonLinearEffectValue;
        DamageBonusPerHit *= LinearEffectValue;
    }

    public override void OnInvokeAbilityUsed(Ability ability)
    {
        if (ability.User != TargetOfEffect || ability.User.InCombat == false) {
            return;
        }
        base.OnInvokeAbilityUsed(ability);
        if(ability is BA_Polearm_F || ability is BA_Polearm_FF || ability is BA_Polearm_FFF) {
            if(HeavySpeed == null || HeavyInjury == null || HeavyStagger == null || HeavySpeed.EffectEnded || HeavyInjury.EffectEnded || HeavyStagger.EffectEnded) {
                HeavySpeed = new Effect_ChangeStat(Player.Instance.HeavyAttackSpeed, SourceOfEffect) {PercentageAmount = SpeedBonusPerHit, DisplayEffectIndicator = true, EffectIndicatorText = Utils.GetFormattedFloat(SpeedBonusPerHit) + "%", PathToEffectGraphic = "UI/HeavyAttackSpeed"};
                HeavyInjury = new Effect_ChangeStat(Player.Instance.HeavyInjury, SourceOfEffect) {PercentageAmount = DamageBonusPerHit};
                HeavyStagger = new Effect_ChangeStat(Player.Instance.HeavyStagger, SourceOfEffect) {PercentageAmount = DamageBonusPerHit};
                Player.Instance.AddEffect(HeavySpeed);
                Player.Instance.AddEffect(HeavyInjury);
                Player.Instance.AddEffect(HeavyStagger);
            }
            else {
                HeavySpeed.PercentageAmount += SpeedBonusPerHit;
                HeavySpeed.EffectIndicatorText = Utils.GetFormattedFloat(HeavySpeed.PercentageAmount) + "%";
                HeavyInjury.PercentageAmount += DamageBonusPerHit;
                HeavyStagger.PercentageAmount += DamageBonusPerHit;
            }
        }
        else if(HeavySpeed != null && HeavyInjury != null && HeavyStagger != null) {
            HeavySpeed.EndThisEffect();
            HeavyInjury.EndThisEffect();
            HeavyStagger.EndThisEffect();
        }
    }
}