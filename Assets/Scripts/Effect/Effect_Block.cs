using System;
using UnityEngine;

public class Effect_Block : Effect {

    public Effect_Block(SourceOfEffect source_of_effect) : base(source_of_effect) {
        Type = EffectType.Buff;
        Listeners.Add(EventManager.AfterHitDamageCalculation);
    }

    public override void OnInvokeAfterHitDamageCalculation(Damage damage) {
        if(damage.TargetOfDamage != TargetOfEffect || damage.CheckIfDamageWorksWithDefensiveAbilities() == false) {
            return;
        }
        if(TargetOfEffect.Actions.CurrentAbilityBeingPerformed != null && (
            TargetOfEffect.Actions.CurrentAbilityBeingPerformed.GetType().IsSubclassOf(typeof(Riposte)) || TargetOfEffect.Actions.CurrentAbilityBeingPerformed.GetType().IsSubclassOf(typeof(Counter))))
        {
            return;
        }
        if (SaveFile.Instance.DifficultyLevel == 0)
        {
            HandleStoryModeBlock(damage);
            base.OnInvokeAfterHitDamageCalculation(damage);
        }
        else
        {
            HandleBlock(damage);
            base.OnInvokeAfterHitDamageCalculation(damage);
        }
    }

    private void HandleStoryModeBlock(Damage damage)
    {
        if (Utils.CheckIfPlayerIsFacingUnit(damage.SourceOfDamage.User) && (damage.SourceOfDamage.IsCounterable))
        {
            PerformRiposteCounter(damage);
        }
        else if(Utils.CheckIfPlayerIsFacingUnit(damage.SourceOfDamage.User) && Player.Instance.IsPerfectlyBlocking && (damage.DamagingObject == null || damage.DamagingObject.CanBeRiposted))
        {
            PerformRiposte(damage);
        }
        else
        {
            PerformBlock(damage);
        }
    }

    private void HandleBlock(Damage damage)
    {
        if (Utils.CheckIfPlayerIsFacingUnit(damage.SourceOfDamage.User) && ((damage.SourceOfDamage.AbilityModifiers.Contains(Constants.AbilityModifier.CounteredByBlock) || (Player.Instance.IsPerfectlyBlocking && damage.SourceOfDamage.AbilityModifiers.Contains(Constants.AbilityModifier.CounteredByRiposte)))))
        {
            PerformRiposteCounter(damage);
        }
        else if (Utils.CheckIfPlayerIsFacingUnit(damage.SourceOfDamage.User) && Player.Instance.IsPerfectlyBlocking && !damage.SourceOfDamage.IsCounterable && (damage.DamagingObject == null || damage.DamagingObject.CanBeRiposted))
        {
            PerformRiposte(damage);
        }
        else if (!damage.SourceOfDamage.IsCounterable)
        {
            PerformBlock(damage);
        }
        else
        {
            damage.Injury *= 0.5f;
            damage.Stagger *= 0.5f;

            TargetOfEffect.AddEffect(new Effect_Stun(SourceOfEffect), SaveFile.Instance.DifficultyLevel < 2 ? 1 : 2);
            TargetOfEffect.AddEffect(new Effect_TakeDecreasedDamage(50, SourceOfEffect) {Type = EffectType.Debuff}, SaveFile.Instance.DifficultyLevel < 2 ? 1 : 2);
        }
    }

    private void PerformRiposteCounter(Damage damage) {
        Utils.PlaySoundEffect(Player.Instance.AudioSource, "Steel/SteelSuperCollision" + UnityEngine.Random.Range(1, 3), 0.65f);
        Vector2 inFrontOfPlayer = Player.Instance.Actions.IsFlipped ? Player.Instance.transform.position + Vector3.left : Player.Instance.transform.position + Vector3.right;
        Utils.CreateVisualEffect(SourceOfEffect, "Counter", inFrontOfPlayer.x, inFrontOfPlayer.y);
        Player.Instance.PerfectBlockSpeed = 1;
        damage.DamageWasRiposted = true;
        damage.DamageWasBlocked = true;
        damage.Injury = 0;
        damage.Stagger = 0;
        Type riposteType = System.Type.GetType(UnitCreatingTheEffect.CurrentWeaponClass.ToString() + "_RiposteCounter");
        int variant = UnityEngine.Random.Range(1, 4);
        Counter counter = (Counter)Activator.CreateInstance(riposteType, new object[] { UnitCreatingTheEffect });
        counter.NameOfAnimationToAutoPlay = UnitCreatingTheEffect.CurrentWeaponClass.ToString() + "_RiposteCounter" + variant;
        counter.Target = damage.SourceOfDamage.User;
        UnitCreatingTheEffect.Actions.CurrentAbilityBeingPerformed = counter;
        if(UnitCreatingTheEffect.Actions.CurrentAbilityBeingPerformed.GeneratedEnergy == false)
        {
            UnitCreatingTheEffect.Energy.GenerateEnergy(Constants.EnergyGainSource.Counter, damage.SourceOfDamage.User.IsBoss);
        }
        EventManager.AbilityWasRipostedOrCountered.Invoke(damage.SourceOfDamage, true);
        damage.SourceOfDamage.User.AddEffect(new Effect_RiposteCountered(SourceOfEffect) {NameOfAnimationToAutoPlay = "RiposteCountered" + variant}, 4f);
        damage.SourceOfDamage.User.Animator.SetFloat("Special Animation Speed", Player.Instance.CurrentWeaponAttackSpeed.Current);
        GameController.Instance.WaitAndRunMethod(1f, Utils.AdjustRemainingCounteredAnimation, damage.SourceOfDamage.User);
        new Damage(damage.SourceOfDamage.User, counter, null)
            .SetDamageSource(0, Constants.STAGGER_PERCENTAGE_FROM_COUNTER, UnitCreatingTheEffect.CurrentWeaponDamageCategory)
            .CalculateDamage();
        if (damage.SourceOfDamage.User is Player || damage.TargetOfDamage is Player) {
            Player.Instance.transform.root.GetComponentInChildren<CameraController>().ShakeScreen(0.2f, 0.1f);
        }
        if(Player.Instance.IsStaggered) {
            Player.Instance.StaggerBar.Current -= Player.Instance.StaggerBar.Maximum * 0.3f;
        }
        EndThisEffect();
    }

    private void PerformRiposte(Damage damage) {
        Utils.PlaySoundEffect(Player.Instance.AudioSource, "Steel/SteelCollision" + UnityEngine.Random.Range(1, 4), 0.4f);
        Vector2 inFrontOfPlayer = Player.Instance.Actions.IsFlipped ? Player.Instance.transform.position + Vector3.left : Player.Instance.transform.position + Vector3.right;
        Utils.CreateVisualEffect(SourceOfEffect, "Riposte", inFrontOfPlayer.x, inFrontOfPlayer.y);
        Player.Instance.PerfectBlockSpeed = 1;
        damage.DamageWasRiposted = true;
        damage.DamageWasBlocked = true;
        damage.Injury = 0;
        damage.Stagger = 0;
        Type riposteType = System.Type.GetType(UnitCreatingTheEffect.CurrentWeaponClass.ToString() + "_Riposte");
        int variant = UnityEngine.Random.Range(1, 6);
        Riposte riposte = (Riposte)Activator.CreateInstance(riposteType, new object[] { UnitCreatingTheEffect });
        riposte.NameOfAnimationToAutoPlay = UnitCreatingTheEffect.CurrentWeaponClass.ToString() + "_Riposte" + variant;
        riposte.Target = damage.SourceOfDamage.User;
        UnitCreatingTheEffect.Actions.CurrentAbilityBeingPerformed = riposte;
        if (UnitCreatingTheEffect.Actions.CurrentAbilityBeingPerformed.GeneratedEnergy == false)
        {
            UnitCreatingTheEffect.Energy.GenerateEnergy(Constants.EnergyGainSource.Riposte, damage.SourceOfDamage.User.IsBoss);
        }
        if (damage.DamagingObject != null && damage.DamagingObject is Projectile) {
            damage.SourceOfDamage.AffectedEnemies.Clear();
            Utils.SendProjectileBackTowardsSource(damage, Player.Instance, riposte);
        }
        else {
            EventManager.AbilityWasRipostedOrCountered.Invoke(damage.SourceOfDamage, false);
            damage.SourceOfDamage.User.AddEffect(new Effect_Riposted(SourceOfEffect) {NameOfAnimationToAutoPlay = "Riposted" + variant}, 3f);
            damage.SourceOfDamage.User.Animator.SetFloat("Special Animation Speed", Player.Instance.CurrentWeaponAttackSpeed.Current);
            GameController.Instance.WaitAndRunMethod(0.5f, AdjustRemainingRipostedAnimation, damage.SourceOfDamage.User);
            new Damage(damage.SourceOfDamage.User, riposte, null)
                .SetDamageSource(0, Constants.STAGGER_PERCENTAGE_FROM_RIPOSTE, UnitCreatingTheEffect.CurrentWeaponDamageCategory)
                .CalculateDamage();
        }
        if (damage.SourceOfDamage.User is Player || damage.TargetOfDamage is Player) {
            Player.Instance.transform.root.GetComponentInChildren<CameraController>().ShakeScreen(0.2f, 0.1f);
        }
        if(Player.Instance.IsStaggered) {
            Player.Instance.StaggerBar.Current -= Player.Instance.StaggerBar.Maximum * 0.1f;
        }
        EndThisEffect();
    }

    private void AdjustRemainingRipostedAnimation(Unit unit) {
        Effect riposted = unit.GetEffect(typeof(Effect_Riposted));
        if(riposted == null || riposted.EffectEnded) {
            return;
        }
        unit.Animator.SetFloat("Special Animation Speed", (3f - (3f * unit.Animator.GetCurrentAnimatorStateInfo(0).normalizedTime)) / riposted.RemainingDuration);
    }

    private void PerformBlock(Damage damage) {
        Utils.PlaySoundEffect(Player.Instance.AudioSource, "Steel/SteelBlock" + UnityEngine.Random.Range(1, 11), 0.4f);
        Vector2 inFrontOfPlayer = Player.Instance.Actions.IsFlipped ? Player.Instance.transform.position + (Vector3.left / 2) : Player.Instance.transform.position + (Vector3.right / 2);
        Utils.CreateVisualEffect(SourceOfEffect, "Block", inFrontOfPlayer.x, inFrontOfPlayer.y);
        damage.DamageWasBlocked = true;
        damage.SourceOfDamage.UpdateAffectedEnemyList(UnitCreatingTheEffect, damage.DamagingObject);
        if(!damage.TargetOfDamage.IsStaggered) {

            damage.Stagger = SaveFile.Instance.DifficultyLevel > 1 ? (damage.Injury * 0.15f + damage.Stagger * 0.6f) : (damage.Injury * 0.1f + damage.Stagger * 0.2f) ;
            damage.Injury = 0;
        }
        else if(damage.TargetOfDamage.IsStaggered) {
            damage.Injury *= SaveFile.Instance.DifficultyLevel > 1 ? 0.4f : 0.1f;
            damage.Stagger *= SaveFile.Instance.DifficultyLevel > 1 ? 0.4f : 0.1f;
        }
        if(SaveFile.Instance.DifficultyLevel == 0) {
            damage.Injury *= 0.5f;
            damage.Stagger *= 0.5f;
        }
        UnitCreatingTheEffect.PlayAnimation(UnitCreatingTheEffect.CurrentWeaponClass + "_BlockSuccessful");
        if (UnitCreatingTheEffect.Actions.CurrentAbilityBeingPerformed.GeneratedEnergy == false)
        {
            UnitCreatingTheEffect.Energy.GenerateEnergy(Constants.EnergyGainSource.Block, damage.SourceOfDamage.User.IsBoss);
        }
        if (damage.SourceOfDamage.User is Player || damage.TargetOfDamage is Player) {
            Player.Instance.transform.root.GetComponentInChildren<CameraController>().ShakeScreen(0.05f, 0.025f);
        }
    }

    public bool CheckIfUserFacingCorrectDirection(Unit enemy_being_blocked)
    {
        return (TargetOfEffect.transform.position.x > enemy_being_blocked.transform.position.x && TargetOfEffect.Actions.IsFlipped) || (TargetOfEffect.transform.position.x <= enemy_being_blocked.transform.position.x && TargetOfEffect.Actions.IsFlipped == false);
    }
}