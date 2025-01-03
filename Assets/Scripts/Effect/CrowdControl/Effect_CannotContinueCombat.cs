using UnityEngine;
using System.Linq;

public class Effect_CannotContinueCombat : Effect_HardCrowdControl
{
    public Effect_CannotContinueCombat(SourceOfEffect source_of_effect) : base(source_of_effect) {
        PriorityLevel = 5;
        Listeners.Add(EventManager.ExitCombat);
        AdditionalEffectsAffectingTargetDuringEffect = new System.Collections.Generic.List<Effect> { new Effect_Invincible(SourceOfEffect) };
    }

    public override void OnStart()
    {
        base.OnStart();
        foreach(Unit u in Utils.GetSpecifiedUnits(new System.Func<Unit, bool>(unit => unit.CurrentTarget == TargetOfEffect))) {
            u.CurrentTarget = u.GetClosestValidTarget();
        }
    }

    public override void OnInvokeExitCombat(Unit unit) {
        if(unit == TargetOfEffect) {
            TargetOfEffect.Health.Current = TargetOfEffect.Health.Maximum;
            EndThisEffect();
        }
    }
}