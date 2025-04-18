using UnityEngine;

public class BA_Polearm_Throw : BasicAttack {


    public BA_Polearm_Throw(Unit ability_user) : base(ability_user) {
        Properties.Add(AbilityProperty.StrongBasicAttack);
        DamageSources.Add(new DamageSource(0, 250, Constants.DamageType.Heavy) {Knockback = 400});
        DamageSources.Add(new DamageSource(0, 50, Constants.DamageType.Heavy, "2"));
        AddCustomSound("Swing1", "Polearm/Polearm_Throw2", 1f);
        AddCustomSound("Swing2", "Polearm/Polearm_Throw1", 1f);
        NameOfAnimationToAutoPlay = SaveFile.Instance.EquippedHeavyWeapon is Polearm_Harpoon ? "Polearm_ThrowAndPull" : "Polearm_Throw";
        TransitionIntoAnimationDuration = 0.02f;
    }

    public override void ExtraBehaviourOnHit(Damage damage)
    {
        if(damage.AbilityDamageSource.ColliderName != "2" && SaveFile.Instance.EquippedHeavyWeapon is Polearm_Harpoon) {
            damage.Stagger += Player.Instance.CurrentStance.Weapon.GetItemFirstEffectPB() * 4.6875f;
        }
        else if(damage.AbilityDamageSource.ColliderName != "2" && SaveFile.Instance.EquippedHeavyWeapon is Polearm_Javelin) {
            damage.Stagger += Player.Instance.CurrentStance.Weapon.GetItemFirstEffectPB() * 3.125f;
            damage.Injury += Player.Instance.CurrentStance.Weapon.GetItemFirstEffectPB() * 3.125f;
        }
    }

    public override void ExtraBehaviourOnDamage(Damage damage)
    {
        if(damage.AbilityDamageSource.ColliderName == "2") {
            Utils.PushUnitIntoPosition(damage.TargetOfDamage, User.Actions.IsFlipped ? (User.transform.position + new Vector3(-2.5f, 0)) : (User.transform.position + new Vector3(2.5f, 0)), this, 30);
        }
        else {
            damage.TargetOfDamage.AddEffect(new Effect_KnockedBack(damage, new(this)));
        }
    }
}