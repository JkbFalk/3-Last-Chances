using UnityEngine;

public class BA_Cannon_F : BasicAttack
{

    public static int AmmoRequiredToUseAbility = 1;

    public BA_Cannon_F(Unit ability_user) : base(ability_user)
    {
        DamageSources.Add(new DamageSource(120, 180, Constants.DamageType.Ranged));
    }

    public override void CallAbilityEvent1()
    {
        if (PlayerControls.BasicAttackButtonHoldDuration > 0.2f)
        {
            User.Actions.CurrentAbilityBeingPerformed = new BA_Cannon_S(User);
        }
    }

    public override void CallAbilityEvent2()
    {
        CanFollowUpAttack = true;
        if (PlayerControls.BasicAttackButtonHoldDuration > 0.2f)
        {
            User.Actions.CurrentAbilityBeingPerformed = new BA_Cannon_S(User);
        }
        else if (PlayerControls.BasicAttackButtonPressCounter > 1)
        {
            Player.Instance.Actions.PerformRegularBasicAttack();
        }
    }

    public override void OnBasicAttackButtonPress()
    {
        base.OnBasicAttackButtonPress();
        if (CanFollowUpAttack)
        {
            Player.Instance.Actions.PerformRegularBasicAttack();
        }
    }
    
    public override void AdditionalAbilitySpecificActionsOnShootingProjectile(Projectile projectile)
    {
        base.AdditionalAbilitySpecificActionsOnShootingProjectile(projectile);
        Utils.PlaySoundEffect(User.AudioSource, "Cannon/Cannon_BasicAttack" + Utils.GetRandomSoundNumber("Cannon_BasicAttack"), 0.6f);
    }
}