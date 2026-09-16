using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Effect_Id : Effect
{
    public Action<Effect_Id> ActionOnStart;
    public Action<Effect_Id> ActionOnEnd;
    public Effect_Id(string identifier, SourceOfEffect source_of_effect) : base(source_of_effect)
    {
    }

    public override void OnStart()
    {
        base.OnStart();
        if (ActionOnStart != null)
        {
            ActionOnStart.Invoke(this);
        }
    }

    public override void OnEnd()
    {
        base.OnEnd();
        if (ActionOnEnd != null)
        {
            ActionOnEnd.Invoke(this);
        }
    }
}
