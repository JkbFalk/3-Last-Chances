using System.Collections.Generic;
using UnityEngine;

public class NPCAbility_FlyingSwordDance: Ability {
    public static float Cooldown = 6;

    public NPCAbility_FlyingSwordDance(Unit ability_user) : base(ability_user) {
    }
}