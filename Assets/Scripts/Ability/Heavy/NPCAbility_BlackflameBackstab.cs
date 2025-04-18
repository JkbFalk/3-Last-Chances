using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NPCAbility_BlackflameBackstab : Ability {
    public static float Cooldown = 15;
    public static AbilityFamily Family = AbilityFamily.Ignis;
    private bool _attackFromRightSide = true;
    private Effect_ChangeStat _coltenSpeedUp;
    private Effect_ChangeStat _coltenPowerUp;
    public NPCAbility_BlackflameBackstab(Unit ability_user) : base(ability_user) {
        WaitTimeBeforeNextAction = 0.2f;
        AddCustomSound("Teleport", "Fire/FlameTeleport", 0.9f);
        Properties.AddRange(new List<Ability.AbilityProperty> {AbilityProperty.ImmuneToFlinch, AbilityProperty.CounteredByBackstep, AbilityProperty.CounteredByRiposte, AbilityProperty.CounteredByBlock, AbilityProperty.CountersBackstep});
        DamageSources.Add(new DamageSource(400, 400, Constants.DamageType.Heavy));
    }

    public override void OnAbilityStart()
    {
        base.OnAbilityStart();
        if(User.CurrentTarget == null) {
            User.CurrentTarget = User.GetClosestValidTarget();
        }
        _attackFromRightSide = User.CurrentTarget.Actions.IsFlipped;
        if(SaveFile.Instance != null && SaveFile.Instance.CurrentMission != null && SaveFile.Instance.CurrentMission is Mission_Ignis3) {
            _coltenSpeedUp = new(User.HeavyAttackSpeed, new(this)) {PercentageModifier = 50 - (SaveFile.Instance.IgnisEnergy / 20)};
            _coltenPowerUp = new(User.HeavyInjury, new(this)) {PercentageModifier = 200 - (SaveFile.Instance.IgnisEnergy / 5)};
            User.AddEffect(_coltenSpeedUp);
            User.AddEffect(_coltenPowerUp);
        }
    }

    public override void OnAbilityEnd()
    {
        base.OnAbilityEnd();
        if(_coltenPowerUp != null && _coltenSpeedUp != null) {
            _coltenPowerUp.EndThisEffect();
            _coltenSpeedUp.EndThisEffect();
        }
    }

    public override void CallAbilityEvent1() {
        if(User.CurrentTarget == null) {
            User.CurrentTarget = User.GetClosestValidTarget();
        }
        if(User.CurrentTarget == null) {
            GameController.Instance.WaitAndRunMethod(0.05f, CallAbilityEvent1);
        }
        else {
            User.Actions.IsFlipped = _attackFromRightSide;
            User.transform.position = _attackFromRightSide ? User.CurrentTarget.transform.position + new Vector3(2.0f, 0) : User.CurrentTarget.transform.position + new Vector3(-2.0f, 0);
        }
    }

    public override void CallAbilityEvent2() {
        ChaseCurrentTargetAtGivenDegreeAngle(100, 20);
    }

    public override void ExtraBehaviourOnDamage(Damage damage)
    {
        base.ExtraBehaviourOnDamage(damage);
        if(SaveFile.Instance != null && SaveFile.Instance.CurrentMission != null && SaveFile.Instance.CurrentMission is Mission_Ignis3) {
            SaveFile.Instance.ChangeIgnisEnergy(-300);
        }
    }
}