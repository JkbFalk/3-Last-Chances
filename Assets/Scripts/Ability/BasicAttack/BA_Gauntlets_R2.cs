using UnityEngine;

public class BA_Gauntlets_R2 : BasicAttack {

    public BA_Gauntlets_R2(Unit ability_user) : base(ability_user)
    {
        Properties.Add(Property.StrongBasicAttack);
        PowerBudget = 240;
        DamageSources.Add(new DamageSource(0, 20, Constants.DamageType.Light));
    }
    public override void CallAbilityEvent1()
    {
        if (PlayerControls.BasicAttackButtonHoldDuration > 0.2f)
        {
            Player.Instance.PerformGauntletStrongBasicAttack();
            
        }
        else if (PlayerControls.BasicAttackButtonPressCounter > 1)
        {
            Player.Instance.PerformGauntletBasicAttack();
        }
        CanFollowUpAttack = true;
    }

    public override void CallAbilityEvent2()
    {
        ChaseCurrentTargetAtGivenDegreeAngle(3, 30);
    }

    public override void OnBasicAttackButtonPress()
    {
        base.OnBasicAttackButtonPress();
        if (CanFollowUpAttack)
        {
            Player.Instance.PerformGauntletBasicAttack();
        }
    }

    public override void ExtraBehaviourOnHit(Damage damage)
    {
        base.ExtraBehaviourOnHit(damage);
        if(damage.TargetOfDamage.CheckIfUnderEffect(typeof(Effect_ProneToKnockout))) {
            Debug.Log(damage.AbilityDamageSource.InjuryScaling);
            Effect_ProneToKnockout prone = (Effect_ProneToKnockout)damage.TargetOfDamage.GetEffect(typeof(Effect_ProneToKnockout));
            damage.AbilityDamageSource = new DamageSource(prone.DecayingAmount, 40 + prone.DecayingAmount, Constants.DamageType.Light);
            Debug.Log(damage.AbilityDamageSource.InjuryScaling);
        }
    }
}