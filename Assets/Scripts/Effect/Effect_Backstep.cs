using System;
using UnityEngine;

public class Effect_Backstep : Effect {

    private bool GenerateEnergyOnSuccessfulDodge = true;
    public Effect_Backstep(SourceOfEffect source_of_effect) : base(source_of_effect) {
        Type = EffectType.Buff;
        Listeners.Add(EventManager.AfterHitDamageCalculation);
    }

    public override void OnStart()
    {
        base.OnStart();
        TargetOfEffect.SpriteRenderers["Lower Body"].Bone.Find("Backstep Hitbox").gameObject.SetActive(true);
    }

    public override void OnEnd()
    {
        base.OnEnd();
        TargetOfEffect.SpriteRenderers["Lower Body"].Bone.Find("Backstep Hitbox").gameObject.SetActive(false);
    }

    public override void OnInvokeAfterHitDamageCalculation(Damage damage) {
        if(damage.TargetOfDamage != TargetOfEffect || damage.CheckIfDamageWorksWithDefensiveAbilities() == false) {
            return;
        }
        if (Utils.CheckIfPlayerIsFacingUnit(damage.SourceOfDamage.User) && (damage.SourceOfDamage.AbilityModifiers.Contains(Constants.AbilityModifier.CounteredByBackstep) || (SaveFile.Instance.DifficultyLevel == 0 && damage.SourceOfDamage.IsCounterable))) {
            damage.DamageWasRiposted = true;
            damage.DamageWasBlocked = true;
            damage.Injury = 0;
            damage.Stagger = 0;
            Type roll_counter_type = System.Type.GetType(UnitCreatingTheEffect.CurrentWeaponClass.ToString() + "_BackstepCounter");
            int variant = UnityEngine.Random.Range(1, 4);
            Counter roll_counter = (Counter)Activator.CreateInstance(roll_counter_type, new object[] { UnitCreatingTheEffect });
            roll_counter.NameOfAnimationToAutoPlay = UnitCreatingTheEffect.CurrentWeaponClass.ToString() + "_BackstepCounter" + variant;
            roll_counter.Target = damage.SourceOfDamage.User;
            UnitCreatingTheEffect.Actions.CurrentAbilityBeingPerformed = roll_counter;
            if (UnitCreatingTheEffect.Actions.CurrentAbilityBeingPerformed.GeneratedEnergy == false)
            {
                UnitCreatingTheEffect.Energy.GenerateEnergy(Constants.EnergyGainSource.Counter, damage.SourceOfDamage.User.IsBoss);
            }
            EventManager.AbilityWasRipostedOrCountered.Invoke(damage.SourceOfDamage, true);
            damage.SourceOfDamage.User.AddEffect(new Effect_BackstepCountered(SourceOfEffect) {NameOfAnimationToAutoPlay = "BackstepCountered" + variant}, 4f);
            damage.SourceOfDamage.User.Animator.SetFloat("Special Animation Speed", Player.Instance.CurrentWeaponAttackSpeed.Current);
            GameController.Instance.WaitAndRunMethod(1f, Utils.AdjustRemainingCounteredAnimation, damage.SourceOfDamage.User);
            new Damage(damage.SourceOfDamage.User, roll_counter, null)
                .SetDamageSource(0, Constants.STAGGER_PERCENTAGE_FROM_COUNTER, UnitCreatingTheEffect.CurrentWeaponDamageCategory)
                .CalculateDamage();
            if (damage.SourceOfDamage.User is Player || damage.TargetOfDamage is Player) {
                Player.Instance.transform.root.GetComponentInChildren<CameraController>().ShakeScreen(0.2f, 0.1f);
            }
            if(Player.Instance.IsStaggered) {
                Player.Instance.StaggerBar.Current -= Player.Instance.StaggerBar.Maximum * 0.3f;
            }
            EndThisEffect();
            base.OnInvokeAfterHitDamageCalculation(damage);
        }
        else if(SaveFile.Instance.DifficultyLevel == 0 || damage.SourceOfDamage.IsCounterable == false)
        {
            float multiplier = 
            !damage.TargetOfDamage.IsStaggered ? 0 : 
            SaveFile.Instance.DifficultyLevel == 0 ? 0 : 
            SaveFile.Instance.DifficultyLevel == 1 ? damage.DamageDealtMultiplier * 0.1f : 
            damage.DamageDealtMultiplier * 0.5f;

            damage.Injury *= multiplier;
            damage.Stagger *= multiplier;

            damage.DecreaseProjectileDurability = false;

            if(GenerateEnergyOnSuccessfulDodge && UnitCreatingTheEffect.Actions.CurrentAbilityBeingPerformed.GeneratedEnergy == false)
            {
                Player.Instance.Energy.GenerateEnergy(Constants.EnergyGainSource.Dodge, damage.SourceOfDamage.User.IsBoss);
            }
            GenerateEnergyOnSuccessfulDodge = false;
            EventManager.DamageWasDodged.Invoke(damage);
            base.OnInvokeAfterHitDamageCalculation(damage);
        }
        else if(SaveFile.Instance.DifficultyLevel > 1 && damage.SourceOfDamage.IsCounterable)
        {
            damage.Injury *= 0.5f;
            damage.Stagger *= 0.5f;

            TargetOfEffect.AddEffect(new Effect_Stun(SourceOfEffect), SaveFile.Instance.DifficultyLevel < 2 ? 1 : 2);
            TargetOfEffect.AddEffect(new Effect_TakeDecreasedDamage(50, SourceOfEffect) {Type = EffectType.Debuff}, SaveFile.Instance.DifficultyLevel < 2 ? 1 : 2);
            base.OnInvokeAfterHitDamageCalculation(damage);
        }
    }
}