using UnityEngine;

public class BA_MagicMelee_F : BasicAttack {
    public BA_MagicMelee_F(Unit ability_user) : base(ability_user) {
        DamageSources.Add(new DamageSource(200, 100, Constants.DamageType.Magic, "AoE") {Knockback = 250});
        DamageTriggerLimit = DamageTriggerLimitType.OncePerUnitFromEachSource;
        AddCustomSound("Swing", "Magic/Magic_Blast2", 1f);
    }

    public override void CallAbilityEvent1()
    {

        if (HoldingMainButton && ButtonPressedCounter == 1)
        {
            User.Actions.CurrentAbilityBeingPerformed = new BA_MagicMelee_S(User);
        }
    }

    public override void CallAbilityEvent2()
    {
        if (HoldingMainButton && ButtonPressedCounter == 2)
        {
            User.Actions.CurrentAbilityBeingPerformed = new BA_MagicMelee_S(User);
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
            User.Actions.CurrentAbilityBeingPerformed = new BA_MagicMelee_S(User);
        }
        else if (ButtonPressedCounter > 2)
        {
            User.Actions.CurrentAbilityBeingPerformed = new BA_MagicMelee_F(User);
        }
    }
}