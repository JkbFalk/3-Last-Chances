using UnityEngine;

public class NPCAbility_SneakJumpAttack : Ability {

    public static float Cooldown = 10;
    public static AbilityFamily Family = AbilityFamily.Salutis;
    public NPCAbility_SneakJumpAttack(Unit ability_user) : base(ability_user) {
        DamageSources.Add(new DamageSource(300, 100, Constants.DamageType.Light));
        WaitTimeBeforeNextAction = 0.1f;
    }

    public override void CallAbilityEvent1() {
        if(User.CurrentTarget == null) {
            return;
        }
        Transform target = User.CurrentTarget.transform;
        Vector2 direction_vector_towards_target = new Vector3(target.position.x + (target.position.x > User.transform.position.x ? -1.1f : 1.1f), target.position.y) - User.transform.position;
        User.ApplyForce(direction_vector_towards_target * 2.5f, this);
    }
}