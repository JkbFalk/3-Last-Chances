using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Effect;

public class Effect_GainTemporaryStatAfterHittingEnemy : Effect
{
    public float BonusDuration = 10;

    public Stat StatAffected;

    public float ScalingAmount = 0;

    public float FlatAmount = 0;

    public float Amount;

    public List<Effect> Buffs = new List<Effect>();

    public Effect_GainTemporaryStatAfterHittingEnemy(Stat stat_affected, SourceOfEffect source_of_effect) : base(source_of_effect)
    {
        Type = EffectType.Buff;
        StatAffected = stat_affected;
        Listeners.Add(EventManager.DamageDealt);
    }

    public void UpdateAffectedStat() {
        StatAffected = Player.Instance.GetUnitStatCorrespondingToGivenStat(StatAffected);
    }

    public override void OnEffectValueChanged()
    {
        Amount = FlatAmount + ScalingAmount * LinearEffectValue;
        FlatAmount = FlatAmount * LinearEffectValue;
        if(StatAffected != null)
        {
            DescriptionParameters = new List<string> { Utils.GetFormattedFloat(Amount), "{StatLabel_" + StatAffected.GetType().ToString() + "}", Utils.GetFormattedFloat(BonusDuration) };
        }
    }

    public override void OnInvokeDamageDealt(Damage damage)
    {
        if(damage.SourceOfDamage.User != TargetOfEffect) {
            return;
        }
        if (damage.SourceOfDamage.AffectedEnemies.Count == 1) {
            Effect_ChangeStat buff = new Effect_ChangeStat(StatAffected, SourceOfEffect) {PercentageAmount = Amount};
            Buffs.Add(buff);
            damage.SourceOfDamage.User.AddEffect(buff, BonusDuration);
            base.OnInvokeDamageDealt(damage);
        }
    }

}
