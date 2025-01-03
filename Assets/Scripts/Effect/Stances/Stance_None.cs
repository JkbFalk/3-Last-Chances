using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stance_None : Effect_Stance
{

    public Stance_None(SourceOfEffect source_of_effect) : base(source_of_effect) {
    }

    public static Ability.AbilityFamily Family = Ability.AbilityFamily.None;

    public override void CreateStanceDisplay() {}
}
