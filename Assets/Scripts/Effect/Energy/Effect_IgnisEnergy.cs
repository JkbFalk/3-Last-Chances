using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Effect_IgnisEnergy : Effect_EnergyUpgrade
{
    public Effect_IgnisEnergy(SourceOfEffect source_of_effect) : base(source_of_effect) {
        Listeners = new List<UnityEventBase> {EventManager.EnemyDefeated, EventManager.HealthBarBroken, EventManager.PlayerObjectReinitialized};
        Family = Ability.AbilityFamily.Ignis;
    }
    public override void OnInvokeEnemyDefeated(Damage damage)
    {
        if(damage.SourceOfDamage.User is Player) {
            base.OnInvokeEnemyDefeated(damage);
            Activate(damage);
        }
    }

    public override void OnInvokeHealthBarBroken(Damage damage)
    {
        if(damage.SourceOfDamage.User is Player) {
            base.OnInvokeHealthBarBroken(damage);
            Activate(damage);
        }
    }

    public void Activate(Damage damage) {
        Player.Instance.AddEffect(new Effect_ChangeStat(Player.Instance.Health, SourceOfEffect) {RegenerationFlatAmount = damage.TargetOfDamage.Health.Maximum / 100}, 10);
        if(UpgradeLevel == 2) {
            int random = UnityEngine.Random.Range(1, 4);
            if(random == 1) {
                float increase_amount = damage.TargetOfDamage.DamageCategory == Constants.DamageType.Heavy ? damage.TargetOfDamage.HeavyInjury.Current / 10 : damage.TargetOfDamage.DamageCategory == Constants.DamageType.Light ? damage.TargetOfDamage.LightInjury.Current / 10 : damage.TargetOfDamage.DamageCategory == Constants.DamageType.Ranged ? damage.TargetOfDamage.RangedInjury.Current / 10 : damage.TargetOfDamage.MagicInjury.Current / 10;
                Player.Instance.AddEffect(new Effect_ChangeCompositeStat(Player.Instance, Effect_ChangeCompositeStat.CompositeStat.Injury, SourceOfEffect) {FlatAmount=increase_amount, DisplayEffectIndicator = true, PathToEffectGraphic = "UI/Injury", EffectIndicatorText=Utils.GetFormattedFloat(increase_amount, 1)}, 20);
            }
            else if(random == 2) {
                float increase_amount = damage.TargetOfDamage.DamageCategory == Constants.DamageType.Heavy ? damage.TargetOfDamage.HeavyStagger.Current / 10 : damage.TargetOfDamage.DamageCategory == Constants.DamageType.Light ? damage.TargetOfDamage.LightStagger.Current / 10 : damage.TargetOfDamage.DamageCategory == Constants.DamageType.Ranged ? damage.TargetOfDamage.RangedStagger.Current / 10 : damage.TargetOfDamage.MagicStagger.Current / 10;
                Player.Instance.AddEffect(new Effect_ChangeCompositeStat(Player.Instance, Effect_ChangeCompositeStat.CompositeStat.Stagger, SourceOfEffect) {FlatAmount=increase_amount, DisplayEffectIndicator = true, PathToEffectGraphic = "UI/Stagger", EffectIndicatorText=Utils.GetFormattedFloat(increase_amount, 1)}, 20);
            }
            else {
                float increase_amount = damage.TargetOfDamage.DamageCategory == Constants.DamageType.Heavy ? damage.TargetOfDamage.HeavyAttackSpeed.Current / 10 : damage.TargetOfDamage.DamageCategory == Constants.DamageType.Light ? damage.TargetOfDamage.LightAttackSpeed.Current / 10 : damage.TargetOfDamage.DamageCategory == Constants.DamageType.Ranged ? damage.TargetOfDamage.RangedAttackSpeed.Current / 10 : damage.TargetOfDamage.MagicAttackSpeed.Current / 10;
                Player.Instance.AddEffect(new Effect_ChangeCompositeStat(Player.Instance, Effect_ChangeCompositeStat.CompositeStat.AttackSpeed, SourceOfEffect) {FlatAmount=increase_amount, DisplayEffectIndicator = true, PathToEffectGraphic = "UI/AttackSpeed", EffectIndicatorText=Utils.GetFormattedFloat(increase_amount, 2)}, 20);
            }
        }
        else if(UpgradeLevel == 3) {
            int multiplier = damage.TargetOfDamage.IsBoss ? 140 : 100;
            if(damage.TargetOfDamage.BaseInjury > damage.TargetOfDamage.BaseStagger && damage.TargetOfDamage.BaseInjury > damage.TargetOfDamage.BaseAttackSpeed * multiplier) {
                float injury = damage.TargetOfDamage.DamageCategory == Constants.DamageType.Heavy ? damage.TargetOfDamage.HeavyInjury.Current / 6.6f : damage.TargetOfDamage.DamageCategory == Constants.DamageType.Light ? damage.TargetOfDamage.LightInjury.Current / 6.6f : damage.TargetOfDamage.DamageCategory == Constants.DamageType.Ranged ? damage.TargetOfDamage.RangedInjury.Current / 6.6f : damage.TargetOfDamage.MagicInjury.Current / 6.6f;
                Player.Instance.AddEffect(new Effect_ChangeCompositeStat(Player.Instance, Effect_ChangeCompositeStat.CompositeStat.Injury, SourceOfEffect) {FlatAmount=injury, DisplayEffectIndicator = true, PathToEffectGraphic = "UI/Injury", EffectIndicatorText=Utils.GetFormattedFloat(injury, 1)}, 20);
            }
            else if(damage.TargetOfDamage.BaseStagger > damage.TargetOfDamage.BaseInjury && damage.TargetOfDamage.BaseStagger > damage.TargetOfDamage.BaseAttackSpeed * multiplier) {
                float stagger = damage.TargetOfDamage.DamageCategory == Constants.DamageType.Heavy ? damage.TargetOfDamage.HeavyStagger.Current / 6.6f : damage.TargetOfDamage.DamageCategory == Constants.DamageType.Light ? damage.TargetOfDamage.LightStagger.Current / 6.6f : damage.TargetOfDamage.DamageCategory == Constants.DamageType.Ranged ? damage.TargetOfDamage.RangedStagger.Current / 6.6f : damage.TargetOfDamage.MagicStagger.Current / 6.6f;
                Player.Instance.AddEffect(new Effect_ChangeCompositeStat(Player.Instance, Effect_ChangeCompositeStat.CompositeStat.Stagger, SourceOfEffect) {FlatAmount=stagger, DisplayEffectIndicator = true, PathToEffectGraphic = "UI/Stagger", EffectIndicatorText=Utils.GetFormattedFloat(stagger, 1)}, 20);
            }
            else {
                float attack_speed = damage.TargetOfDamage.DamageCategory == Constants.DamageType.Heavy ? damage.TargetOfDamage.HeavyAttackSpeed.Current / 6.6f : damage.TargetOfDamage.DamageCategory == Constants.DamageType.Light ? damage.TargetOfDamage.LightAttackSpeed.Current / 6.6f : damage.TargetOfDamage.DamageCategory == Constants.DamageType.Ranged ? damage.TargetOfDamage.RangedAttackSpeed.Current / 6.6f : damage.TargetOfDamage.MagicAttackSpeed.Current / 6.6f;
                Player.Instance.AddEffect(new Effect_ChangeCompositeStat(Player.Instance, Effect_ChangeCompositeStat.CompositeStat.AttackSpeed, SourceOfEffect) {FlatAmount=attack_speed, DisplayEffectIndicator = true, PathToEffectGraphic = "UI/AttackSpeed", EffectIndicatorText=Utils.GetFormattedFloat(attack_speed, 2)}, 20);
            }
        }
    }
}
