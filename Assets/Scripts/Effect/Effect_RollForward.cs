using System;
using UnityEngine;
using UnityEngine.AI;

public class Effect_RollForward : Effect {
    public Effect_RollForward(SourceOfEffect source_of_effect) : base(source_of_effect) {
        Type = EffectType.Buff;
        Listeners.Add(EventManager.AfterHitDamageCalculation);
    }

    public override void OnStart()
    {
        base.OnStart();
        TargetOfEffect.GetComponent<NavMeshObstacle>().enabled = false;
    }

    public override void OnEnd() {
        base.OnEnd();
        TargetOfEffect.GetComponent<NavMeshObstacle>().enabled = true;
    }

    public override void OnInvokeAfterHitDamageCalculation(Damage damage) {
        if(damage.TargetOfDamage != TargetOfEffect || damage.CheckIfDamageWorksWithDefensiveAbilities() == false) {
            return;
        }
        if (Utils.CheckIfPlayerIsFacingUnit(damage.SourceOfDamage.User) && damage.SourceOfDamage.Is(Ability.AbilityProperty.CounteredByRoll) || (SaveFile.Instance.DifficultyLevel == 0 && damage.SourceOfDamage.Is(Ability.AbilityProperty.Counter)))
        {
            damage.DamageWasRiposted = true;
            damage.DamageWasBlocked = true;
            damage.Injury = 0;
            damage.Stagger = 0;
            Type roll_counter_type = System.Type.GetType(UnitCreatingTheEffect.CurrentWeaponClass.ToString() + "_RollCounter");
            int variant = UnityEngine.Random.Range(1, 4);
            Counter roll_counter = (Counter)Activator.CreateInstance(roll_counter_type, new object[] { UnitCreatingTheEffect });
            roll_counter.NameOfAnimationToAutoPlay = UnitCreatingTheEffect.CurrentWeaponClass.ToString() + "_RollCounter" + variant;
            roll_counter.Target = damage.SourceOfDamage.User;
            UnitCreatingTheEffect.Actions.CurrentAbilityBeingPerformed = roll_counter;
            if (UnitCreatingTheEffect.Actions.CurrentAbilityBeingPerformed.IsNot(Ability.AbilityProperty.AlreadyGeneratedEnergy))
            {
                UnitCreatingTheEffect.Energy.GenerateEnergy(Constants.EnergyGainSource.Counter, damage.SourceOfDamage.User.IsBoss);
                UnitCreatingTheEffect.Actions.CurrentAbilityBeingPerformed.Properties.Add(Ability.AbilityProperty.AlreadyGeneratedEnergy);
            }
            EventManager.AbilityWasRipostedOrCountered.Invoke(damage.SourceOfDamage, true);
            damage.SourceOfDamage.User.AddEffect(new Effect_RollCountered(SourceOfEffect) {NameOfAnimationToAutoPlay = "RollCountered" + variant}, 4f);
            damage.SourceOfDamage.User.Animator.SetFloat("Special Animation Speed", Player.Instance.CurrentWeaponAttackSpeed.Current);
            GameController.Instance.WaitAndRunMethod(1f, Utils.AdjustRemainingCounteredAnimation, damage.SourceOfDamage.User);
            new Damage(damage.SourceOfDamage.User, roll_counter, null)
                .SetDamageSource(0,  Constants.STAGGER_PERCENTAGE_FROM_COUNTER, UnitCreatingTheEffect.CurrentWeaponDamageCategory)
                .CalculateDamage();
            if (damage.SourceOfDamage.User is Player || damage.TargetOfDamage is Player)
            {
                CameraController.Instance.ShakeScreen(0.2f, 0.1f);
            }
            if(Player.Instance.IsStaggered) {
                Player.Instance.StaggerBar.Current -= Player.Instance.StaggerBar.Maximum * 0.3f;
            }
            EndThisEffect();
            base.OnInvokeAfterHitDamageCalculation(damage);
        }
        else if (SaveFile.Instance.DifficultyLevel == 0 || damage.SourceOfDamage.Is(Ability.AbilityProperty.Counter) == false)
        {
            float multiplier = 
            !damage.TargetOfDamage.IsStaggered ? 0 : 
            SaveFile.Instance.DifficultyLevel == 0 ? 0 : 
            SaveFile.Instance.DifficultyLevel == 1 ? damage.DamageDealtMultiplier * 0.1f : 
            damage.DamageDealtMultiplier * 0.5f;

            damage.Injury *= multiplier;
            damage.Stagger *= multiplier;

            damage.DecreaseProjectileDurability = false;

            if (UnitCreatingTheEffect.Actions.CurrentAbilityBeingPerformed.IsNot(Ability.AbilityProperty.AlreadyGeneratedEnergy))
            {
                Player.Instance.Energy.GenerateEnergy(Constants.EnergyGainSource.Dodge, damage.SourceOfDamage.User.IsBoss);
                UnitCreatingTheEffect.Actions.CurrentAbilityBeingPerformed.Properties.Add(Ability.AbilityProperty.AlreadyGeneratedEnergy);
            }
            EventManager.DamageWasDodged.Invoke(damage);
            base.OnInvokeAfterHitDamageCalculation(damage);
        }
        else if (SaveFile.Instance.DifficultyLevel > 0 && damage.SourceOfDamage.Is(Ability.AbilityProperty.Counter))
        {
            damage.Injury *= 0.5f;
            damage.Stagger *= 0.5f;

            TargetOfEffect.AddEffect(new Effect_Stun(SourceOfEffect), SaveFile.Instance.DifficultyLevel < 2 ? 1.5f : 3);
            TargetOfEffect.AddEffect(new Effect_ChangeStat(Player.Instance.DamageReduction, SourceOfEffect) {PercentageAmount = 50, Type = EffectType.Debuff}, SaveFile.Instance.DifficultyLevel < 2 ? 1.5f : 3);
            base.OnInvokeAfterHitDamageCalculation(damage);
        }
    }
}