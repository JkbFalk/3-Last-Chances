using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Effect_GainASForDistancelTravelled : Effect
{
    private float[] _distanceTravelledInGivenSecond = new float[25] {0, 0, 0, 0, 0,0, 0, 0, 0, 0,0, 0, 0, 0, 0,0, 0, 0, 0, 0,0, 0, 0, 0, 0};
    public float ASPer1MTravelled;
    public Vector2 _prevPosition;
    private Effect_ChangeCompositeStat ASBuff;
    public Effect_GainASForDistancelTravelled(float as_per_1m_travelled, SourceOfEffect source_of_effect) : base(source_of_effect)
    {
        Type = EffectType.Buff;
        ASPer1MTravelled = as_per_1m_travelled;
        Listeners.Add(EventManager.OneFifthSecondElapsedInGame);
    }

    public override void OnStart()
    {
        base.OnStart();
        _prevPosition = TargetOfEffect.transform.position;
        ASBuff = new Effect_ChangeCompositeStat(TargetOfEffect, Effect_ChangeCompositeStat.CompositeStat.AttackSpeed, SourceOfEffect) {PercentageModifier = 0, ShowsInUI=true, PathToEffectGraphic="UI/AttackSpeed", EffectIndicatorText="0%"};
        TargetOfEffect.AddEffect(ASBuff);
    }

    public override void OnEnd()
    {
        base.OnEnd();
        ASBuff.EndThisEffect();
    }

    public override void OnInvokeOneFifthSecondElapsedInGame()
    {
        float[] _updatedDistances = new float[25];
        _updatedDistances[0] = Vector2.Distance(TargetOfEffect.transform.position, _prevPosition);
        for(int i = 1; i < 25; i++) {
            _updatedDistances[i] = _distanceTravelledInGivenSecond[i - 1];
        }
        _distanceTravelledInGivenSecond = _updatedDistances;
        _prevPosition = TargetOfEffect.transform.position;
        float travelledTotal = 0;
        for(int i = 0; i < 25; i++) {
            travelledTotal += _distanceTravelledInGivenSecond[i] * (1 - 0.04f * i);
        }
        ASBuff.PercentageModifier = travelledTotal * ASPer1MTravelled;
        ASBuff.EffectIndicatorText = Utils.GetFormattedFloat(ASBuff.PercentageModifier) + "%";
    }

}
