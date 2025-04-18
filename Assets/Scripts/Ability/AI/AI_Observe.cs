using System.Collections.Generic;
using UnityEngine;

public class AI_Observe : AI {
    public AI_Observe(Unit ability_user, Unit target) : base(ability_user, target) {
        CanInterruptCurrentAbility = true;
        User.UnitAI.CurrentAIBehavior = Constants.AIBehavior.Observing;
        User.Actions.EndCurrentAbility();
    }

    public override void OnAbilityStart()
    {
        base.OnAbilityStart();
        int random = UnityEngine.Random.Range(1, 100);
        float randomSpeed = UnityEngine.Random.Range(0.6f, 1) + User.UnitAI.Aggressiveness / 5;
        if (random < 50 && User.DamageType != Constants.DamageType.Magic)
        {
            User.Actions.FaceUnit(User.CurrentTarget);
            User.Animator.SetFloat("Observe Speed", UnityEngine.Random.Range(0.6f, 1f) + User.UnitAI.Aggressiveness / 5);
            User.PlayAnimation("Observe_" + User.DamageType + UnityEngine.Random.Range(1, 5));
            GameController.Instance.WaitAndRunMethod(4 / randomSpeed, InstantlyAttack);
            
        }
        else if (random < 90)
        {
            User.Actions.FaceUnit(User.CurrentTarget);
            User.Animator.SetFloat("Observe Speed", UnityEngine.Random.Range(0.6f, 1f) + User.UnitAI.Aggressiveness / 5);
            User.PlayAnimation("Observe_None" + UnityEngine.Random.Range(1, 5));
            GameController.Instance.WaitAndRunMethod(4 / randomSpeed, InstantlyAttack);
        }
        else {
            User.PlayAnimation("IdleInCombat");
        }
    }

    public void InstantlyAttack() {
        if(SaveFile.Instance.DifficultyLevel > 1 && User.UnitAI != null && User.InCombat && Utils.CheckIfUnitCanPerformActions(User)) {
            User.UnitAI.DecideOnNextAction(true);
        }
    }
}