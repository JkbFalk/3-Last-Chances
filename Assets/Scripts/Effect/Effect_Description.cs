using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Effect_Description : Effect
{
    public float PercentageAmount { get; set; } = 0;
    public Effect_Description(SourceOfEffect source_of_effect) : base(source_of_effect)
    {
    }
}
