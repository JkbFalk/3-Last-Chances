using UnityEngine;

public class Effect_RiposteCountered : Effect_HardCrowdControl
{
    public Effect_RiposteCountered(SourceOfEffect source_of_effect) : base(source_of_effect) {
        CanBeNegatedByImmunityToCrowdControl = false;
        PriorityLevel = 8;
        ScaleWithControlAndTenacity = false;
        //AdditionalEffectsAffectingTargetDuringEffect = new System.Collections.Generic.List<Effect> { new Effect_RootedInPlace(source_of_effect) };
    }

    public override void OnStart()
    {
        base.OnStart();
        SourceOfEffect.User.Rigidbody2D.velocity = Vector2.zero;
        TargetOfEffect.Rigidbody2D.velocity = Vector2.zero;
        Utils.PushUnitIntoPosition(TargetOfEffect, new Vector2(SourceOfEffect.User.transform.position.x + (SourceOfEffect.User.transform.position.x > TargetOfEffect.transform.position.x ? -Constants.DISTANCE_AWAY_FROM_COUNTERING_UNIT : Constants.DISTANCE_AWAY_FROM_COUNTERING_UNIT), SourceOfEffect.User.transform.position.y), SourceOfEffect.SourceAbility, 50);
    }
}