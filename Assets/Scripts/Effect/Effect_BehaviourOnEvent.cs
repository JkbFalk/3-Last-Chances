using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class Effect_BehaviourOnEvent : Effect
{
    public float PercentageAmount = 0;
    public float FlatAmount = 0;
    public Func<bool> ConditionCheckForEvent;
    public Action<Effect_BehaviourOnEvent> ActionOnEvent;
    public UnityEventBase EventBase;
    private UnityEvent UnityEvent;
    public List<System.Object> EventParameters = new();

    public Effect_BehaviourOnEvent(UnityEventBase unity_event, List<System.Object> event_parameters, SourceOfEffect source_of_effect) : base(source_of_effect) {
        Type = EffectType.Buff;
        EventBase = unity_event;
        EventParameters = event_parameters;
    }

    public override void OnStart() {
        base.OnStart();
        UnityEvent.AddListener(OnInvokeCustomBehaviour);
    }

    public override void OnEnd() {
        base.OnEnd();
        UnityEvent.RemoveListener(OnInvokeCustomBehaviour);
    }

    public void OnInvokeCustomBehaviour()
    {
        if (ConditionCheckForEvent != null && ConditionCheckForEvent.Invoke())
        {
            ActionOnEvent.Invoke(this);
        }
    }
}
