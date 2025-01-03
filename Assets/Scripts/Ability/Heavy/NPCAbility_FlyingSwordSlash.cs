using System.Collections.Generic;
using UnityEngine;

public class NPCAbility_FlyingSwordSlash: Ability {
    public static float Cooldown = 6;

    public NPCAbility_FlyingSwordSlash(Unit ability_user) : base(ability_user) {
    }
}