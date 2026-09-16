using Unity.VisualScripting;
using UnityEngine;

public class NPCAbility_ShieldCharge : Ability {
    public static float Cooldown = 7;
    public NPCAbility_ShieldCharge(Unit ability_user) : base(ability_user) {
        DamageSources.Add(new DamageSource(10, 200, Constants.DamageType.Heavy) {KnockbackInMeters = 3f, CustomHitSound="Ability/Criminal_Shield_MoveAttack"});
        DamageSources.Add(new DamageSource(10, 50, Constants.DamageType.Heavy, "2+") {KnockbackInMeters = 3f, CustomHitSound="Ability/Criminal_Shield_MoveAttack"});
        DamageSources.Add(new DamageSource(10, 50, Constants.DamageType.Heavy, "ChargeAoE") {KnockbackInMeters = 3f, CustomHitSound="Ability/Criminal_Shield_MoveAttack"});
        WaitTimeBeforeNextAction = 0.2f;
        PlaySoundOnlyOnce = true;
        HitSoundVolume = 0.7f;
        AddCustomSound("Charge", "Criminal/Criminal_Shield_Charge", 0.4f);
        Properties.Add(Property.ImmuneToFlinch);
    }

    public override void CallAbilityEvent1() {
        PlayCustomSound("Charge");
        ResetPotentialTargets();
        Vector2 direction_vector_towards_target = CombatMath.GetDirectionVector(User.transform.position, User.CurrentTarget != null ? User.CurrentTarget.transform.position : User.Actions.IsFlipped ? User.transform.position + Vector3.left : User.transform.position + Vector3.right, User.Actions.IsFlipped, 45);
        User.PushInTargetDirection(direction_vector_towards_target * 4, this);
    }

    public override void CallAbilityEvent2()
    {
        Transform aoe = User.SpriteRenderers["Lower Body"].Bone.Find("AreaOfEffect_ChargeAoE");
        if(aoe != null && aoe.gameObject.IsDestroyed() == false) {
            aoe.GetComponent<TemporaryObject>().MakeObjectDisappear();
        }
    }
}