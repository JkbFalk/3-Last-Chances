using System.Collections.Generic;
using UnityEngine;

public class NPCAbility_FlyingSwordBarrage: Ability {
    public static float Cooldown = 6;

    public NPCAbility_FlyingSwordBarrage(Unit ability_user) : base(ability_user) {
    }
}