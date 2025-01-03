using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Effect_CannotBeDefeated : Effect
{
    
    public bool GrantsExperienceOnDefeat;
    public Effect_CannotBeDefeated(bool grants_experience_on_defeat, SourceOfEffect source_of_effect) : base(source_of_effect) {
        GrantsExperienceOnDefeat = grants_experience_on_defeat;
    }
}
