using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using System.Linq;

public class Stance_PlunderingFlame : Effect_Stance
{
    public static Ability.AbilityFamily Family = Ability.AbilityFamily.Ignis;

    private TextMeshProUGUI _amountDisplay;
    public static float EnergyGenerated = 10;
    public static float BurnAmplifiedPercentage = 30;
    public static float CooldownRefund = 30;
    public static float CooldownRefundAgainstBosses = 90;
    public Effect_ChangeEffectPower IncreaseBurn;

    public Stance_PlunderingFlame(SourceOfEffect source_of_effect) : base(source_of_effect) {
        Listeners = new List<UnityEventBase> { EventManager.DamageDealt, EventManager.EffectStarted, EventManager.EffectEmpowered};
        
    }

    public override void OnStanceActivated()
    {
        IncreaseBurn = new(typeof(Effect_Burn), Effect_ChangeEffectPower.ChangeTypeEnum.AffectAmountAdded, Effect_ChangeEffectPower.AffectedUnitsTypeEnum.Enemies, 30, SourceOfEffect) {ShowsInMenu=false};
        Player.Instance.AddEffect(IncreaseBurn);
    }

    public override void OnStanceDeactivated()
    {
        IncreaseBurn.EndThisEffect();
    }

    public override void OnInvokeEffectEmpowered(Effect existing_effect, Effect new_effect) {
        if(IsActive && new_effect is Effect_Burn && new_effect.SourceOfEffect.User is Player && new_effect.TargetOfEffect.IsHostile) {
            Player.Instance.Health.Current += ((Effect_Burn)new_effect).DecayingAmount;
        }
    }

    public override void OnInvokeEffectStarted(Effect effect)
    {
        base.OnInvokeEffectStarted(effect);
        if(IsActive && effect is Effect_Burn && effect.SourceOfEffect.User is Player && effect.TargetOfEffect.IsHostile) {
            Player.Instance.Health.Current += ((Effect_Burn)effect).DecayingAmount;
        }
        if(IsActive && UnlockedUpgrade2 && effect is Effect_Burn && effect.SourceOfEffect.User is Player && effect.TargetOfEffect.IsHostile) {
            Player.Instance.Energy.GenerateEnergy(effect.TargetOfEffect.IsBoss ? EnergyGenerated * 3 : EnergyGenerated);
        }
        else if(IsActive && UnlockedUpgrade3 && effect is Effect_Staggered && effect.SourceOfEffect.User is Player && effect.TargetOfEffect.CheckIfUnderEffect(typeof(Effect_Burn)) && effect.TargetOfEffect.IsHostile) {
            foreach(Cooldown cd in Player.Instance.AbilityCooldowns.Concat(Player.Instance.EffectCooldowns).Concat(new List<Cooldown> {Player.Instance.ItemsCooldown}))
            {
                if(cd != null && cd.RemainingDuration > 0)
                {
                    cd.RemainingDuration -= cd.TotalDuration * (effect.TargetOfEffect.IsBoss ? 0.9f : 0.3f);
                    if(cd.RemainingDuration < 0) {
                        cd.RemainingDuration = 0;
                    }
                }
            }
        }
    }

    public override void CreateStanceDisplay() {
        base.CreateStanceDisplay();
        _amountDisplay = Player.Instance.CurrentStanceGauge.transform.Find("Gauge/Amount").GetComponent<TextMeshProUGUI>();
    }

    public static List<string> GetDescriptionValues()
    {
        return new List<string> {"250"};
    }

    public static List<string> GetDescriptionUpgrade1Values()
    {
        return new List<string> {BurnAmplifiedPercentage.ToString()};
    }

    public static List<string> GetDescriptionUpgrade2Values()
    {
        return new List<string> {EnergyGenerated.ToString()};
    }

    public static List<string> GetDescriptionUpgrade3Values()
    {
        return new List<string> {CooldownRefund.ToString(), CooldownRefundAgainstBosses.ToString()};
    }
}
