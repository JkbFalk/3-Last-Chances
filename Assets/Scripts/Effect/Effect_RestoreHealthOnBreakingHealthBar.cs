using System.Collections.Generic;
using UnityEngine;

public class Effect_RestoreHealthOnBreakingHealthBar : Effect
{
    //Value = 25 flat hp / 100p
    public float HealthRestoreAmount = 0;
    public bool RestoresPercentageAmount = false;

    public Effect_RestoreHealthOnBreakingHealthBar(bool is_percentage, SourceOfEffect source_of_effect) : base(source_of_effect)
    {
        Type = EffectType.Buff;
        RestoresPercentageAmount = is_percentage;
        HealthRestoreAmount = is_percentage ? 0.2f : 6.25f;
        Listeners.Add(EventManager.EnemyDefeated);
        Listeners.Add(EventManager.HealthBarBroken);
    }

    public override void OnInvokeEnemyDefeated(Damage damage)
    {
        Activate(damage);
        base.OnInvokeEnemyDefeated(damage);
    }

    public override void OnInvokeHealthBarBroken(Damage damage)
    {
        Activate(damage);
        base.OnInvokeEnemyDefeated(damage);
    }

    public override void OnEffectValueChanged()
    {
        HealthRestoreAmount *= LinearEffectValue;
        DescriptionParameters = new List<string> { Utils.GetFormattedFloat(HealthRestoreAmount) };
    }

    public void Activate(Damage damage)
    {
        damage.SourceOfDamage.User.Health.Current += RestoresPercentageAmount ? damage.SourceOfDamage.User.Health.Current * HealthRestoreAmount / 100 : HealthRestoreAmount;
    }
}