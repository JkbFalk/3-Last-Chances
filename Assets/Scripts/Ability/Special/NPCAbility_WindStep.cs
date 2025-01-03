using System.Collections.Generic;
using UnityEngine;

public class NPCAbility_WindStep : Ability {

    public static float Cooldown = 10;
    public static AbilityFamily Family = AbilityFamily.Anima;
    public NPCAbility_WindStep(Unit ability_user) : base(ability_user) {
        AddCustomSound("Jump", "Footsteps/Footsteps_Earth1", 0.8f);
        AddCustomSound("Wind", "Wind/WindSlash", 0.5f);
        WaitTimeBeforeNextAction = 0;
    }

    public override void CallAbilityEvent1()
    {
        float distance = Vector2.Distance(User.transform.position, User.CurrentTarget.transform.position);
        ChaseCurrentTargetAtGivenDegreeAngle(distance * 50, 85, distance * 4f);
    }

    public override void OnAbilityEnd()
    {
        base.OnAbilityEnd();
        GameController.Instance.WaitAndRunMethod(0.5f / User.UnitAI.Aggressiveness, Attack);
    }

    public void Attack() {
        User.UnitAI.DecideOnNextAction(true);
    }

    public static bool CheckIfSpecialConditionsAreFulfilled(Unit user)
    {
        if(user.CurrentTarget == null) {
            return false;
        }
        float distance = Vector2.Distance(user.transform.position, user.CurrentTarget.transform.position);
        return distance > 3.5f;
    }
}