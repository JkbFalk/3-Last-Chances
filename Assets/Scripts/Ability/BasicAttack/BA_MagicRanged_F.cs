using UnityEngine;
public class BA_MagicRanged_F : BasicAttack {
    public BA_MagicRanged_F(Unit ability_user) : base(ability_user) {
        DamageSources.Add(new DamageSource(100, 50, Constants.DamageType.Magic) {Knockback = 100});
        DamageTriggerLimit = DamageTriggerLimitType.OncePerUnitFromEachSource;
        AddCustomSound("Swing", "Magic/Magic_Blast3", 0.6f);
    }

    public override void CallAbilityEvent1()
    {

        if (HoldingMainButton && ButtonPressedCounter == 1)
        {
            User.Actions.CurrentAbilityBeingPerformed = new BA_MagicRanged_S(User);
        }
    }

    public override void CallAbilityEvent2()
    {
        if (HoldingMainButton && ButtonPressedCounter == 2)
        {
            User.Actions.CurrentAbilityBeingPerformed = new BA_MagicRanged_S(User);
        }
        else if (ButtonPressedCounter <= 1)
        {
            EndThisAbility();
        }
    }

    public override void CallAbilityEvent3()
    {
        if (HoldingMainButton && ButtonPressedCounter == 3)
        {
            User.Actions.CurrentAbilityBeingPerformed = new BA_MagicRanged_S(User);
        }
        else if (ButtonPressedCounter > 2)
        {
            User.Actions.CurrentAbilityBeingPerformed = new BA_MagicRanged_F(User);
        }
    }
}