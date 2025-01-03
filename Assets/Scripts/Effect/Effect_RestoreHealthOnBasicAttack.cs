using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Effect_RestoreHealthOnBasicAttack : Effect {
    public float HealthRestoreAmount = 0;
    public bool RestoresPercentageAmount = false;

    public Effect_RestoreHealthOnBasicAttack(SourceOfEffect source_of_effect) : base(source_of_effect) {
        Type = EffectType.Buff;
        TriggersOncePerAbility = true;
        Listeners.Add(EventManager.DamageDealt);
    }

    public override void OnEffectValueChanged()
    {
        HealthRestoreAmount *= LinearEffectValue;
        DescriptionParameters = new List<string> { Utils.GetFormattedFloat(HealthRestoreAmount) };
    }

    public override void OnInvokeDamageDealt(Damage damage)
    {
        if (damage.SourceOfDamage.User == TargetOfEffect && damage.SourceOfDamage.IsBasicAttack && damage.SourceOfDamage.TriggeredEffects.Contains(this) == false && (damage.InjuryDealt > 0 || damage.StaggerDealt > 0))
        {
            damage.SourceOfDamage.User.Health.Current += RestoresPercentageAmount ? damage.SourceOfDamage.User.Health.Current * HealthRestoreAmount / 100 : HealthRestoreAmount;
            base.OnInvokeDamageDealt(damage);
        }
    }
}