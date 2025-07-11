using UnityEngine;

public class BA_MagicMelee_F : BasicAttack {
    public BA_MagicMelee_F(Unit ability_user) : base(ability_user) {
        DamageSources.Add(new DamageSource(200, 100, Constants.DamageType.Magic, "AoE") {KnockbackInMeters = 2.5f});
        DamageTriggerLimit = DamageTriggerLimitType.OncePerUnitFromEachSource;
        AddCustomSound("Swing", "Magic/Magic_Blast2", 1f);
    }

    public override void CallAbilityEvent1()
    {

        if (PlayerControls.BasicAttackButtonHoldDuration > Constants.MINIMUM_HOLD_DURATION_FOR_STRONG_BASIC_ATTACKS)
        {
            User.Actions.CurrentAbilityBeingPerformed = new BA_MagicMelee_S(User);
        }
    }

    public override void CallAbilityEvent2()
    {
        if (PlayerControls.BasicAttackButtonHoldDuration > Constants.MINIMUM_HOLD_DURATION_FOR_STRONG_BASIC_ATTACKS)
        {
            User.Actions.CurrentAbilityBeingPerformed = new BA_MagicMelee_S(User);
        }
        else
        {
            EndThisAbility();
        }
    }

    public override void CallAbilityEvent3()
    {
        if (PlayerControls.BasicAttackButtonHoldDuration > Constants.MINIMUM_HOLD_DURATION_FOR_STRONG_BASIC_ATTACKS)
        {
            User.Actions.CurrentAbilityBeingPerformed = new BA_MagicMelee_S(User);
        }
        else
        {
            User.Actions.CurrentAbilityBeingPerformed = new BA_MagicMelee_F(User);
        }
    }
}