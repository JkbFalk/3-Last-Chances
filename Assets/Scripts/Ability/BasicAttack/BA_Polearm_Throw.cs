using UnityEngine;

public class BA_Polearm_Throw : BasicAttack {


    public BA_Polearm_Throw(Unit ability_user) : base(ability_user) {
        Properties.Add(Property.StrongBasicAttack);
        DamageSources.Add(new DamageSource(0, 250, Constants.DamageType.Heavy) {KnockbackInMeters = 4f});
        DamageSources.Add(new DamageSource(0, 50, Constants.DamageType.Heavy, "2"));
        AddCustomSound("Swing1", "Polearm/Polearm_Throw2", 1f);
        AddCustomSound("Swing2", "Polearm/Polearm_Throw1", 1f);
        NameOfAnimationToAutoPlay = Player.Instance.CheckIfUnderEffectWithGivenId("ReplaceAllBasicAttacksWithThrowAddPullAndIncreaseStagger") ? "Polearm_ThrowAndPull" : "Polearm_Throw";
        TransitionIntoAnimationDuration = 0.02f;
    }

    public override void ExtraBehaviourOnHit(DamageInstance damage)
    {
        if(damage.AbilityDamageSource.ColliderName != "2" && Player.Instance.CheckIfUnderEffectWithGivenId("ReplaceAllBasicAttacksWithThrowAddPullAndIncreaseStagger")) {
            damage.Stagger += Player.Instance.CurrentStance.Weapon.GetItemFirstEffectPB() * 4.6875f;
        }
        else if(damage.AbilityDamageSource.ColliderName != "2" && Player.Instance.CheckIfUnderEffectWithGivenId("ReplaceAllBasicAttacksWithThrowAndIncreaseInjury")) {
            damage.Stagger += Player.Instance.CurrentStance.Weapon.GetItemFirstEffectPB() * 3.125f;
            damage.Injury += Player.Instance.CurrentStance.Weapon.GetItemFirstEffectPB() * 3.125f;
        }
    }

    public override void ExtraBehaviourOnDamage(DamageInstance damage)
    {
        if(damage.AbilityDamageSource.ColliderName == "2") {
            damage.TargetOfDamage.PushIntoPosition(User.Actions.IsFlipped ? (User.transform.position + new Vector3(-2.5f, 0)) : (User.transform.position + new Vector3(2.5f, 0)), this, 1.05f);
        }
        else {
            damage.TargetOfDamage.AddEffect(new Effect_KnockedBack(damage, new(this)));
        }
    }
}