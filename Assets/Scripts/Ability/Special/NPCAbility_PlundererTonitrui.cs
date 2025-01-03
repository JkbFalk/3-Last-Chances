using System.Collections.Generic;
using UnityEngine;

public class NPCAbility_PlundererTonitrui : Ability {

    public static AbilityFamily Family = AbilityFamily.Tonitrui;
    private Vector2 _savedPosition;
    public NPCAbility_PlundererTonitrui(Unit ability_user) : base(ability_user) {
        DamageSources.Add(new DamageSource(300, 400, Constants.DamageType.Heavy));
        AddCustomSound("Charge", "Ability/Ability_ThunderStrike_Charge", 0.9f);
        AddCustomSound("Hit", "Ability/Ability_ThunderStrike_Regular", 0.9f);
        CanBeInterruptedByFlinching = false;
        TransitionIntoAnimationDuration = 0;
        EffectsAffectingUserDuringAbility = new List<Effect>() { new Effect_Immovable(new(this)), new Effect_RootedInPlace(new(this)) };
    }

    public override void OnAbilityStart()
    {
        base.OnAbilityStart();
        User.Actions.FaceCurrentTarget();
    }

    public override void ExtraBehaviourOnDamage(Damage damage)
    {
        damage.TargetOfDamage.AddEffect(new Effect_Stun(new(this)), 5);
    }

    public override void CallAbilityEvent1() {
        PlayCustomSound("Charge");
        _savedPosition = User.CurrentTarget.transform.position;
    }

    public override void CallAbilityEvent2() {
        PlayCustomSound("Hit");
        AreaOfEffect aoe = Utils.CreateAreaOfEffect(new(this), "PlundererTonitrui");
        aoe.transform.parent.position = _savedPosition;
    }

}