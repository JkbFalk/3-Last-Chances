using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Effect;

public class Effect_GainExtraAmmoOnFinishingOffEnemies : Effect
{
    public int AmmoAmount = 1;
    public Constants.DamageType DamageType;

    public Effect_GainExtraAmmoOnFinishingOffEnemies(Constants.DamageType damage_type, SourceOfEffect source_of_effect) : base(source_of_effect)
    {
        Type = EffectType.Buff;
        DamageType = damage_type;
        Listeners.Add(EventManager.EnemyDefeated);
        Listeners.Add(EventManager.HealthBarBroken);
    }

    public override void OnInvokeEnemyDefeated(Damage damage)
    {
        base.OnInvokeEnemyDefeated(damage);
        Activate(damage);
    }

    public override void OnInvokeHealthBarBroken(Damage damage)
    {
        base.OnInvokeEnemyDefeated(damage);
        Activate(damage);
    }

    public override void OnEffectValueChanged()
    {
        AmmoAmount = LinearEffectValue == 20 ? 1 : LinearEffectValue == 40 ? 2 : LinearEffectValue == 80 ? 3 : 0;
        DescriptionParameters = new List<string> { Label.Get("WeaponCategory_" + DamageType), Utils.GetFormattedFloat(AmmoAmount)};
    }

    public void Activate(Damage damage)
    {
        if(damage.SourceOfDamage.User == TargetOfEffect && damage.AbilityDamageSource.DamageType == DamageType)
        {
            Player.Instance.Ammo += AmmoAmount;
        }
    }
}
