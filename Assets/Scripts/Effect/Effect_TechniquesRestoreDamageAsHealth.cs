using System.Collections.Generic;
using UnityEngine;

public class Effect_EnergyAbilitiesRestoreDamageAsHealth : Effect {
        public float HealthRestorePercentage = 10;
    public float MaxAmount = 30;
    Dictionary<Technique, float> HealingPerAbility = new Dictionary<Technique, float>();

    public override string ToString()
    {
        if(MaxAmount == 9999) {
            return Label.Get("Effect_EnergyAbilitiesRestoreDamageAsHealthNoLimit_DescriptionSimple");
        }
        else {
            return base.ToString();
        }
    }

    public Effect_EnergyAbilitiesRestoreDamageAsHealth(float percentage, float max_amount, SourceOfEffect source_of_effect) : base(source_of_effect) {
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
        if (damage.SourceOfDamage.User != TargetOfEffect || !damage.SourceOfDamage.IsTechnique || (damage.InjuryDealt <= 0 && damage.StaggerDealt <= 0)) {
            return;
        }
        Technique sourceOfDamage = (Technique)damage.SourceOfDamage;
        float alreadyHealed = HealingPerAbility.ContainsKey(sourceOfDamage) ? HealingPerAbility[sourceOfDamage] : 0;
        if(alreadyHealed == 0 || HealingPerAbility[sourceOfDamage] < MaxAmount) {
            float healing = damage.InjuryDealt * HealthRestorePercentage / 100;
            if(alreadyHealed + healing > MaxAmount ) {
                healing = MaxAmount - alreadyHealed;
            }
            damage.SourceOfDamage.User.Health.Current += healing;
            HealingPerAbility[sourceOfDamage] = alreadyHealed + healing;
        }
        base.OnInvokeDamageDealt(damage);
    }
}