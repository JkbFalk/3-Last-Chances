using UnityEngine;
public class BA_MagicRanged_F : BasicAttack {
    public BA_MagicRanged_F(Unit ability_user) : base(ability_user) {
        DamageSources.Add(new DamageSource(100, 50, Constants.DamageType.Magic) {KnockbackInMeters = 1f});
        DamageTriggerLimit = DamageTriggerLimitType.OncePerUnitFromEachSource;
        AddCustomSound("Swing", "Magic/Magic_Blast3", 0.6f);
    }

    public override void CallAbilityEvent1()
    {

        if (PlayerControls.BasicAttackButtonHoldDuration > Constants.MINIMUM_HOLD_DURATION_FOR_STRONG_BASIC_ATTACKS)
        {
            User.Actions.CurrentAbilityBeingPerformed = new BA_MagicRanged_S(User);
        }
    }

    public override void CallAbilityEvent2()
    {
        if (PlayerControls.BasicAttackButtonHoldDuration > Constants.MINIMUM_HOLD_DURATION_FOR_STRONG_BASIC_ATTACKS)
        {
            User.Actions.CurrentAbilityBeingPerformed = new BA_MagicRanged_S(User);
        }
        else if (PlayerControls.BasicAttackButtonPressCounter <= 1)
        {
            EndThisAbility();
        }
    }

    public override void CallAbilityEvent3()
    {
        if (PlayerControls.BasicAttackButtonHoldDuration > Constants.MINIMUM_HOLD_DURATION_FOR_STRONG_BASIC_ATTACKS)
        {
            User.Actions.CurrentAbilityBeingPerformed = new BA_MagicRanged_S(User);
        }
        else if (PlayerControls.BasicAttackButtonPressCounter > 2)
        {
            User.Actions.CurrentAbilityBeingPerformed = new BA_MagicRanged_F(User);
        }
    }
}