using System.Collections.Generic;
using UnityEngine;

public class Effect_BARestoreDamageAsHealth : Effect {
    public float HealthRestorePercentage = 10;
    public float MaxAmount = 5;

    public override string ToString()
    {
        if(MaxAmount == 9999) {
            return Label.Get("Effect_BARestoreDamageAsHealthNoLimit_DescriptionSimple");
        }
        else {
            return base.ToString();
        }
    }

    public Effect_BARestoreDamageAsHealth(float percentage, float max_amount, SourceOfEffect source_of_effect) : base(source_of_effect) {
        Type = EffectType.Buff;
        TriggersOncePerAbility = true;
        HealthRestorePercentage = percentage;
        MaxAmount = max_amount;
        Listeners.Add(EventManager.DamageDealt);
    }

    public override void OnEffectValueChanged()
    {
        MaxAmount *= LinearEffectValue;
        DescriptionParameters = new List<string> { Utils.GetFormattedFloat(HealthRestorePercentage), Utils.GetFormattedFloat(MaxAmount) };
    }

    public override void OnInvokeDamageDealt(Damage damage)
    {
        if(damage.SourceOfDamage.User != TargetOfEffect) {
            return;
        }
        if (damage.SourceOfDamage.IsBasicAttack && damage.SourceOfDamage.TriggeredEffects.Contains(this) == false && (damage.InjuryDealt > 0 || damage.StaggerDealt > 0))
        {
            damage.SourceOfDamage.User.Health.Current += damage.InjuryDealt * HealthRestorePercentage / 100 > MaxAmount ? MaxAmount : damage.InjuryDealt * HealthRestorePercentage / 100;
            base.OnInvokeDamageDealt(damage);
        }
    }
}