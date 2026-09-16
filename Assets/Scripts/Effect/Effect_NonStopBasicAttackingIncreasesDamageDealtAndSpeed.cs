using System;
using UnityEngine;

public class Effect_NonStopBasicAttackingIncreasesDamageDealtAndSpeed : Effect {
    //Value = 10 dmg or stagger / 100p

    private Effect_ChangeStat _heavySpeed;
    private Effect_ChangeStat _heavyInjury;
    private Effect_ChangeStat _heavyStagger;
    public float AttackSpeedBonusPerHit;
    public float DamageBonusPerHit;

    public Effect_NonStopBasicAttackingIncreasesDamageDealtAndSpeed(SourceOfEffect source_of_effect) : base(source_of_effect) {
        Type = EffectType.Buff;
        Listeners.Add(EventManager.AbilityUsed);
    }

    public override void OnInvokeAbilityUsed(Ability ability)
    {
        if (ability.User != TargetOfEffect || ability.User.InCombat == false) {
            return;
        }
        base.OnInvokeAbilityUsed(ability);
        if(ability is BA_Polearm_F || ability is BA_Polearm_FF || ability is BA_Polearm_FFF) {
            if(_heavySpeed == null || _heavyInjury == null || _heavyStagger == null || _heavySpeed.EffectEnded || _heavyInjury.EffectEnded || _heavyStagger.EffectEnded) {
                _heavySpeed = new Effect_ChangeStat(Player.Instance.HeavyAttackSpeed, SourceOfEffect) {
                    PercentageAmount = AttackSpeedBonusPerHit, 
                    ShowsInUI = true, 
                    UIText = Utils.GetFormattedFloat(AttackSpeedBonusPerHit, 0), 
                    PathToUIGraphic = "UI/HeavyAttackSpeed"
                };
                _heavyInjury = new Effect_ChangeStat(Player.Instance.HeavyInjury, SourceOfEffect) {
                    PercentageAmount = DamageBonusPerHit
                };
                _heavyStagger = new Effect_ChangeStat(Player.Instance.HeavyStagger, SourceOfEffect) {
                    PercentageAmount = DamageBonusPerHit
                };
                Player.Instance.AddEffect(_heavySpeed);
                Player.Instance.AddEffect(_heavyInjury);
                Player.Instance.AddEffect(_heavyStagger);
            }
            else {
                _heavySpeed.PercentageAmount += AttackSpeedBonusPerHit;
                _heavySpeed.UIText = Utils.GetFormattedFloat(_heavySpeed.PercentageAmount, 0);
                _heavyInjury.PercentageAmount += DamageBonusPerHit;
                _heavyStagger.PercentageAmount += DamageBonusPerHit;
            }
        }
        else if(_heavySpeed != null && _heavyInjury != null && _heavyStagger != null) {
            _heavySpeed.EndThisEffect();
            _heavyInjury.EndThisEffect();
            _heavyStagger.EndThisEffect();
        }
    }
}