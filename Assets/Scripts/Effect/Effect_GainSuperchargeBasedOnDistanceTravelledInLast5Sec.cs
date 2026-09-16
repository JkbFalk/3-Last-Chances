using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Effect_PerformActionBasedOnDistanceTravelledInLast5Sec : Effect
{
    private float[] _distanceTravelledInGivenTenthSecond = new float[50] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
    public float MetersTravelledInLast5Seconds;
    public Vector2 _prevPosition;
    public Effect_PerformActionBasedOnDistanceTravelledInLast5Sec(SourceOfEffect source_of_effect) : base(source_of_effect)
    {
        Type = EffectType.Buff;
        Listeners.Add(EventManager.OneTenthSecondElapsedInGame);
    }
    public override void OnInvokeOneTenthSecondElapsedInGame()
    {
        float[] _updatedDistances = new float[50];
        _updatedDistances[0] = Vector2.Distance(TargetOfEffect.transform.position, _prevPosition);
        for (int i = 1; i < 50; i++)
        {
            _updatedDistances[i] = _distanceTravelledInGivenTenthSecond[i - 1];
        }
        _distanceTravelledInGivenTenthSecond = _updatedDistances;
        _prevPosition = TargetOfEffect.transform.position;
        MetersTravelledInLast5Seconds = 0;
        for (int i = 0; i < 50; i++)
        {
            MetersTravelledInLast5Seconds += _distanceTravelledInGivenTenthSecond[i] * (1 - 0.04f * i);
        }
        Action.Invoke(this);
    }
    
    public Action<Effect_PerformActionBasedOnDistanceTravelledInLast5Sec> Action; 
}
