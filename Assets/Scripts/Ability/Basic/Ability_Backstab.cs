using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Backstab : Ability {

    public Backstab(Unit ability_user) : base(ability_user) {
        EffectsAffectingUserDuringAbility = new List<Effect> { new Effect_Invincible(new(this))};
    }
}
