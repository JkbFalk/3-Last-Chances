using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Effect_GainMoreEnergyFromSpecifiedSource : Effect { 
    public float IncreaseAmount = 0;
    public Constants.EnergyGainSource SpecifiedSource;
    public Effect_GainMoreEnergyFromSpecifiedSource(SourceOfEffect source_of_effect) : base(source_of_effect) {
        Type = EffectType.Buff;
    }

    public override void OnStart()
    {
        base.OnStart();
        EventManager.GeneratedEnergy.AddListener(GainMoreEnergy);
    }

    public override void OnEnd()
    {
        base.OnEnd();
        EventManager.GeneratedEnergy.RemoveListener(GainMoreEnergy);
    }

    public void GainMoreEnergy(float base_gain, float energy, Constants.EnergyGainSource source) {
        if(source == SpecifiedSource) {
            Player.Instance.Energy.Current += (base_gain * IncreaseAmount / 100);
        }
    }
}
