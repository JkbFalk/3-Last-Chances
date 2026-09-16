using UnityEngine;

public class NPCAbility_SmashAndShoot : Ability {
    public static float Cooldown = 6;
    public static AbilityFamily Family = AbilityFamily.Ignis;
    public NPCAbility_SmashAndShoot(Unit ability_user) : base(ability_user) {
        DamageSources.Add(new DamageSource(0, 350, Constants.DamageType.Heavy));
        DamageSources.Add(new DamageSource(250, 150, Constants.DamageType.Ranged, "FireShot") {KnockbackInMeters = 5.5f});
        AddCustomSound("Swing", "Heavy Object/HeavyObject_Swing1", 0.4f);
        AddCustomSound("Shoot", "Explosion/CannonBallShot", 0.65f);
        WaitTimeBeforeNextAction = 0.3f;
        DamageTriggerLimit = DamageTriggerLimitType.OncePerUnitFromEachSource;
        Properties.Add(Property.CounteredByRoll);
    }

    public override void CallAbilityEvent1() {
        ChaseCurrentTargetAtGivenDegreeAngle(7, 35);
    }

    public override void CallAbilityEvent2()
    {
        Properties.Clear();
    }

    public override void ExtraBehaviourOnDamage(DamageInstance damage)
    {
        if(damage.DamagingObject.gameObject.name == "AoE") {
            damage.TargetOfDamage.AddEffect(new Effect_Burn(50 * User.MagicStagger.Current / 100, new(this)));
            damage.TargetOfDamage.AddEffect(new Effect_KnockedBack(damage, new(this)));
            GameObject vfx = Utils.CreateVisualEffect(new(this), "FireShotExplosion", damage.TargetOfDamage.transform.position.x, damage.TargetOfDamage.transform.position.y);
        }
        else {
            damage.TargetOfDamage.AddEffect(new Effect_Stun(new(this)), 0.7f);
            damage.TargetOfDamage.PushInTargetDirection(User.Actions.IsFlipped ? Vector2.left * 9 : Vector2.right * 9, this);
        }
    }
}