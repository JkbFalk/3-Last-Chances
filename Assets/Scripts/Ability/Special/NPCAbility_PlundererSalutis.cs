using System.Collections.Generic;
using UnityEngine;

public class NPCAbility_PlundererSalutis : Ability {
    public static AbilityFamily Family = AbilityFamily.Salutis;
    public NPCAbility_PlundererSalutis(Unit ability_user) : base(ability_user) {
        DamageSources.Add(new DamageSource(500, 0, Constants.DamageType.Heavy));
        AddCustomSound("Disappear", "Wind/WindWhoosh1", 0.9f);
        AddCustomSound("Appear", "Wind/WindWhoosh2", 0.9f);
        TransitionIntoAnimationDuration = 0;
        Properties.AddRange(new List<Property> {Property.ImmuneToFlinch, Property.CountersBlock, Property.CountersRiposte});
    }

    public override void OnAbilityStart()
    {
        base.OnAbilityStart();
        User.Actions.FaceCurrentTarget();
    }

    public override void ExtraBehaviourOnDamage(DamageInstance damage)
    {
        damage.TargetOfDamage.AddEffect(new Effect_Prone(50, new(this)), 10);
    }

    public override void CallAbilityEvent1() {
        PlayCustomSound("Swing");
        User.transform.position = Target.transform.position + (Target.Actions.IsFlipped ? Vector3.right : Vector3.left);
        User.Actions.IsFlipped = Target.Actions.IsFlipped;
    }

}