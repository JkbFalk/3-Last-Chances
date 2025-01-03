using System.Collections.Generic;
using UnityEngine;

public abstract class AI : Ability {

    public AI(Unit ability_user, Unit target) : base(ability_user) {
        User.Actions.EndCurrentAbility();
        CanBePlundered = false;
        AutoPlayAbilityAnimation = false;
    }
}