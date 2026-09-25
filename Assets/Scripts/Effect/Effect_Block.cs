// FILE: Assets\Scripts\Effect\Effect_Block.cs
using System;
using UnityEngine;

public class Effect_Block : Effect 
{
    public bool CanStopBlocking = false;

    public Effect_Block(SourceOfEffect source_of_effect) : base(source_of_effect) 
    {
        Type = EffectType.Buff;
        Listeners.Add(EventManager.AfterHitDamageCalculation);
    }

    public override void OnInvokeAfterHitDamageCalculation(DamageInstance damage) 
    {
        if (damage.TargetOfDamage != TargetOfEffect || damage.CheckIfInteractsWithCounters() == false) return;

        if (TargetOfEffect.Actions.CurrentAbilityBeingPerformed != null && (
            TargetOfEffect.Actions.CurrentAbilityBeingPerformed.GetType().IsSubclassOf(typeof(Riposte)) || 
            TargetOfEffect.Actions.CurrentAbilityBeingPerformed.GetType().IsSubclassOf(typeof(Counter))))
        {
            return;
        }

        if (SaveFile.Instance.DifficultyLevel == 0) {
            HandleStoryModeBlock(damage);
        } else {
            HandleBlock(damage);
        }
        base.OnInvokeAfterHitDamageCalculation(damage);
    }

    private void HandleStoryModeBlock(DamageInstance damage)
    {
        if (CombatMath.CheckIfPlayerIsFacingUnit(damage.SourceOfDamage.User) && damage.SourceOfDamage.Is(Ability.Property.Counter)) {
            PerformRiposteCounter(damage);
        }
        else if (CombatMath.CheckIfPlayerIsFacingUnit(damage.SourceOfDamage.User) && Player.Instance.IsPerfectlyBlocking && (damage.DamagingObject == null || damage.DamagingObject.CanBeRiposted)) {
            EvaluateParryOrRiposte(damage);
        }
        else {
            PerformBlock(damage);
        }
    }

    private void HandleBlock(DamageInstance damage)
    {
        if (CombatMath.CheckIfPlayerIsFacingUnit(damage.SourceOfDamage.User) && (damage.SourceOfDamage.Is(Ability.Property.CounteredByBlock) || (Player.Instance.IsPerfectlyBlocking && damage.SourceOfDamage.Is(Ability.Property.CounteredByRiposte)))) {
            PerformRiposteCounter(damage);
        }
        else if (CombatMath.CheckIfPlayerIsFacingUnit(damage.SourceOfDamage.User) && Player.Instance.IsPerfectlyBlocking && !damage.SourceOfDamage.Is(Ability.Property.Counter) && (damage.DamagingObject == null || damage.DamagingObject.CanBeRiposted)) {
            EvaluateParryOrRiposte(damage);
        }
        else if (!damage.SourceOfDamage.Is(Ability.Property.Counter)) {
            PerformBlock(damage);
        }
        else {
            // Guard Crush / Failed Block penalty
            damage.Injury *= 0.5f;
            damage.Stagger *= 0.5f;
            TargetOfEffect.AddEffect(new Effect_Stun(SourceOfEffect), SaveFile.Instance.DifficultyLevel < 2 ? 1.5f : 3);
            TargetOfEffect.AddEffect(new Effect_ChangeStat(Player.Instance.Armor, SourceOfEffect) {PercentageAmount = 50, Type = EffectType.Debuff}, SaveFile.Instance.DifficultyLevel < 2 ? 1.5f : 3);
        }
    }

    private void EvaluateParryOrRiposte(DamageInstance damage)
    {
        damage.SourceOfDamage.RipostedCount++;

        // Calculate a rough projection of the Stagger damage this parry will deal
        float effectiveStagger = Player.Instance.GetStaggerStatForGivenDamageType(UnitCreatingTheEffect.CurrentWeaponDamageType).Current;
        float projectedStaggerDamage = (Constants.STAGGER_PERCENTAGE_FROM_PARRY / 100f) * effectiveStagger; 

        // If the parry will break the stagger bar, OR if it's the final hit of the enemy's combo, execute a FULL Riposte.
        bool isBarBreak = (damage.SourceOfDamage.User.StaggerBar.Current + projectedStaggerDamage) >= damage.SourceOfDamage.User.StaggerBar.Maximum;
        bool isFinalHit = damage.SourceOfDamage.RipostedCount >= damage.SourceOfDamage.RequiredParriesForRiposte;

        if (isBarBreak || isFinalHit) {
            PerformFullRiposte(damage);
        } else {
            PerformQuickParry(damage);
        }
    }

    private void PerformQuickParry(DamageInstance damage)
    {
        Utils.PlaySoundEffect(Player.Instance.AudioSource, "Steel/SteelCollision" + UnityEngine.Random.Range(1, 4), 0.35f); // Quieter than full riposte
        Vector2 inFrontOfPlayer = Player.Instance.Actions.IsFlipped ? Player.Instance.transform.position + Vector3.left : Player.Instance.transform.position + Vector3.right;
        Utils.CreateVisualEffect(SourceOfEffect, "Riposte", inFrontOfPlayer.x, inFrontOfPlayer.y);
        
        Player.Instance.PerfectBlockSpeed = 1;
        damage.DamageWasRiposted = true;
        damage.DamageWasBlocked = true;
        damage.Injury = 0;
        damage.Stagger = 0;

        Type riposteType = AbilityTypeRegistry.GetRiposte(UnitCreatingTheEffect.CurrentWeaponClass);
        if (riposteType == null) return;

        int variant = UnityEngine.Random.Range(1, 4); // E.g., Greatsword_Parry1, Greatsword_Parry2
        Riposte parryAbility = (Riposte)Activator.CreateInstance(riposteType, new object[] { UnitCreatingTheEffect });
        
        // Call the new Parry animations instead of the Riposte animations
        parryAbility.NameOfAnimationToAutoPlay = UnitCreatingTheEffect.CurrentWeaponClass.ToString() + "_Parry" + variant;
        parryAbility.Target = damage.SourceOfDamage.User;
        parryAbility.OriginalRipostedAbility = damage.SourceOfDamage.User.Actions.CurrentAbilityBeingPerformed;
        
        // Scale down the damage for a mid-combo parry
        if (parryAbility.DamageSources.Count > 0) {
            parryAbility.DamageSources[0].InjuryScaling = Constants.INJURY_PERCENTAGE_FROM_PARRY;
            parryAbility.DamageSources[0].StaggerScaling = Constants.STAGGER_PERCENTAGE_FROM_PARRY;
        }

        // CRITICAL: Allow the player to immediately block or dodge the next hit in the combo!
        parryAbility.CanAlwaysBeInterruptedBy.Add(Ability.AbilityInterruptType.Block);
        parryAbility.CanAlwaysBeInterruptedBy.Add(Ability.AbilityInterruptType.Dodge);

        UnitCreatingTheEffect.Actions.CurrentAbilityBeingPerformed = parryAbility;

        if (UnitCreatingTheEffect.Actions.CurrentAbilityBeingPerformed.IsNot(Ability.Property.AlreadyGeneratedEnergy)) {
            UnitCreatingTheEffect.Energy.GenerateEnergy(Constants.EnergyGainSource.Riposte, damage.SourceOfDamage.User.IsBoss);
            UnitCreatingTheEffect.Actions.CurrentAbilityBeingPerformed.Properties.Add(Ability.Property.AlreadyGeneratedEnergy);
        }

        // Projectiles get deflected back but don't interrupt the boss
        if (damage.DamagingObject != null && damage.DamagingObject is Projectile) {
            damage.SourceOfDamage.AffectedEnemies.Clear();
            Utils.SendProjectileBackTowardsSource(damage, Player.Instance, parryAbility);
        } else {
            EventManager.AbilityWasRipostedOrCountered.Invoke(damage.SourceOfDamage, false);
            
            // Notice we do NOT add Effect_Riposted here. The enemy keeps swinging!
            new DamageInstance(damage.SourceOfDamage.User, parryAbility, null)
                .SetDamageSource(0, Constants.STAGGER_PERCENTAGE_FROM_PARRY, UnitCreatingTheEffect.CurrentWeaponDamageType)
                .CalculateAndApplyDamage();
        }

        if (damage.SourceOfDamage.User is Player || damage.TargetOfDamage is Player) {
            CameraController.Instance.ShakeScreen(0.1f, 0.05f); // Mild screen shake
        }
        
        EndThisEffect();
    }

    private void PerformFullRiposte(DamageInstance damage) 
    {
        Utils.PlaySoundEffect(Player.Instance.AudioSource, "Steel/SteelCollision" + UnityEngine.Random.Range(1, 4), 0.7f);
        Vector2 inFrontOfPlayer = Player.Instance.Actions.IsFlipped ? Player.Instance.transform.position + Vector3.left : Player.Instance.transform.position + Vector3.right;
        Utils.CreateVisualEffect(SourceOfEffect, "Riposte", inFrontOfPlayer.x, inFrontOfPlayer.y);
        
        Player.Instance.PerfectBlockSpeed = 1;
        damage.DamageWasRiposted = true;
        damage.DamageWasBlocked = true;
        damage.Injury = 0;
        damage.Stagger = 0;

        Type riposteType = AbilityTypeRegistry.GetRiposte(UnitCreatingTheEffect.CurrentWeaponClass);
        if (riposteType == null) return;

        int variant = UnityEngine.Random.Range(1, 4);
        Riposte riposte = (Riposte)Activator.CreateInstance(riposteType, new object[] { UnitCreatingTheEffect });
        riposte.NameOfAnimationToAutoPlay = UnitCreatingTheEffect.CurrentWeaponClass.ToString() + "_Riposte" + variant;
        riposte.Target = damage.SourceOfDamage.User;
        riposte.OriginalRipostedAbility = damage.SourceOfDamage.User.Actions.CurrentAbilityBeingPerformed;
        UnitCreatingTheEffect.Actions.CurrentAbilityBeingPerformed = riposte;

        if (UnitCreatingTheEffect.Actions.CurrentAbilityBeingPerformed.IsNot(Ability.Property.AlreadyGeneratedEnergy)) {
            UnitCreatingTheEffect.Energy.GenerateEnergy(Constants.EnergyGainSource.Riposte, damage.SourceOfDamage.User.IsBoss);
            UnitCreatingTheEffect.Actions.CurrentAbilityBeingPerformed.Properties.Add(Ability.Property.AlreadyGeneratedEnergy);
        }

        if (damage.DamagingObject != null && damage.DamagingObject is Projectile) {
            damage.SourceOfDamage.AffectedEnemies.Clear();
            Utils.SendProjectileBackTowardsSource(damage, Player.Instance, riposte);
        } else {
            EventManager.AbilityWasRipostedOrCountered.Invoke(damage.SourceOfDamage, false);
            
            // Apply the actual stun lock
            damage.SourceOfDamage.User.AddEffect(new Effect_Riposted(SourceOfEffect) {NameOfAnimationToAutoPlay = "Riposted" + variant}, 3f);
            damage.SourceOfDamage.User.Animator.SetFloat("Special Animation Speed", Player.Instance.CurrentWeaponAttackSpeed.Current);
            GameController.Instance.WaitAndRunMethod(0.5f, AdjustRemainingRipostedAnimation, damage.SourceOfDamage.User);
            
            new DamageInstance(damage.SourceOfDamage.User, riposte, null)
                .SetDamageSource(0, Constants.STAGGER_PERCENTAGE_FROM_RIPOSTE, UnitCreatingTheEffect.CurrentWeaponDamageType)
                .CalculateAndApplyDamage();
        }

        if (damage.SourceOfDamage.User is Player || damage.TargetOfDamage is Player) {
            CameraController.Instance.ShakeScreen(0.2f, 0.1f); // Heavy screen shake
        }
        
        EndThisEffect();
    }

    private void AdjustRemainingRipostedAnimation(Unit unit) {
        Effect riposted = unit.GetEffect(typeof(Effect_Riposted));
        if(riposted == null || riposted.EffectEnded) return;
        unit.Animator.SetFloat("Special Animation Speed", (3f - (3f * unit.Animator.GetCurrentAnimatorStateInfo(0).normalizedTime)) / riposted.RemainingDuration);
    }

    private void PerformRiposteCounter(DamageInstance damage) 
    {
        // (Unchanged existing Counter logic)
        Utils.PlaySoundEffect(Player.Instance.AudioSource, "Steel/SteelSuperCollision" + UnityEngine.Random.Range(1, 3), 0.65f);
        Vector2 inFrontOfPlayer = Player.Instance.Actions.IsFlipped ? Player.Instance.transform.position + Vector3.left : Player.Instance.transform.position + Vector3.right;
        Utils.CreateVisualEffect(SourceOfEffect, "Counter", inFrontOfPlayer.x, inFrontOfPlayer.y);
        Player.Instance.PerfectBlockSpeed = 1;
        damage.DamageWasRiposted = true;
        damage.DamageWasBlocked = true;
        damage.Injury = 0;
        damage.Stagger = 0;
        Type riposteType = AbilityTypeRegistry.GetRiposteCounter(UnitCreatingTheEffect.CurrentWeaponClass);
        if (riposteType == null) return;

        int variant = UnityEngine.Random.Range(1, 4);
        Counter counter = (Counter)Activator.CreateInstance(riposteType, new object[] { UnitCreatingTheEffect });
        counter.NameOfAnimationToAutoPlay = UnitCreatingTheEffect.CurrentWeaponClass.ToString() + "_RiposteCounter" + variant;
        counter.Target = damage.SourceOfDamage.User;
        counter.OriginalRipostedAbility = damage.SourceOfDamage.User.Actions.CurrentAbilityBeingPerformed;
        UnitCreatingTheEffect.Actions.CurrentAbilityBeingPerformed = counter;

        if(UnitCreatingTheEffect.Actions.CurrentAbilityBeingPerformed.IsNot(Ability.Property.AlreadyGeneratedEnergy)) {
            UnitCreatingTheEffect.Energy.GenerateEnergy(Constants.EnergyGainSource.Counter, damage.SourceOfDamage.User.IsBoss);
            UnitCreatingTheEffect.Actions.CurrentAbilityBeingPerformed.Properties.Add(Ability.Property.AlreadyGeneratedEnergy);
        }

        EventManager.AbilityWasRipostedOrCountered.Invoke(damage.SourceOfDamage, true);
        damage.SourceOfDamage.User.AddEffect(new Effect_RiposteCountered(SourceOfEffect) {NameOfAnimationToAutoPlay = "RiposteCountered" + variant}, 4f);
        damage.SourceOfDamage.User.Animator.SetFloat("Special Animation Speed", Player.Instance.CurrentWeaponAttackSpeed.Current);
        GameController.Instance.WaitAndRunMethod(1f, Utils.AdjustRemainingCounteredAnimation, damage.SourceOfDamage.User);
        
        new DamageInstance(damage.SourceOfDamage.User, counter, null)
            .SetDamageSource(0, Constants.STAGGER_PERCENTAGE_FROM_COUNTER, UnitCreatingTheEffect.CurrentWeaponDamageType)
            .CalculateAndApplyDamage();

        if (damage.SourceOfDamage.User is Player || damage.TargetOfDamage is Player) {
            CameraController.Instance.ShakeScreen(0.3f, 0.15f);
        }

        EndThisEffect();
    }

    private void PerformBlock(DamageInstance damage) 
    {
        Utils.PlaySoundEffect(Player.Instance.AudioSource, "Steel/SteelBlock" + UnityEngine.Random.Range(1, 11), 0.4f);
        Vector2 inFrontOfPlayer = Player.Instance.Actions.IsFlipped ? Player.Instance.transform.position + (Vector3.left / 2) : Player.Instance.transform.position + (Vector3.right / 2);
        Utils.CreateVisualEffect(SourceOfEffect, "Block", inFrontOfPlayer.x, inFrontOfPlayer.y);
        damage.SourceOfDamage.UpdateAffectedEnemyList(UnitCreatingTheEffect, damage.DamagingObject);
        
        if(!damage.TargetOfDamage.IsStaggered) {
            damage.Stagger = SaveFile.Instance.DifficultyLevel > 1 ? (damage.Injury * 0.15f + damage.Stagger * 0.6f) : (damage.Injury * 0.1f + damage.Stagger * 0.2f) ;
            damage.Injury = 0;
        } else {
            damage.Injury *= SaveFile.Instance.DifficultyLevel > 1 ? 0.5f : 0.1f;
            damage.Stagger *= SaveFile.Instance.DifficultyLevel > 1 ? 0.5f : 0.1f; 
        }

        if(SaveFile.Instance.DifficultyLevel == 0) {
            damage.Injury *= 0.5f;
            damage.Stagger *= 0.5f;
        }

        UnitCreatingTheEffect.PlayAnimation(UnitCreatingTheEffect.CurrentWeaponClass + "_BlockSuccessful");
        UnitCreatingTheEffect.Energy.GenerateEnergy(Constants.EnergyGainSource.Block, damage.SourceOfDamage.User.IsBoss);
        
        if (damage.SourceOfDamage.User is Player || damage.TargetOfDamage is Player) {
            CameraController.Instance.ShakeScreen(0.05f, 0.025f);
        }
    }
}