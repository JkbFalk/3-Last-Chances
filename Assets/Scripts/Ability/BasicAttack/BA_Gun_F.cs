using System.Linq;
using UnityEngine;

public class BA_Gun_F : BasicAttack {
    public static int AmmoRequiredToUseAbility = 1;
    private bool _shotBullet = false;
    public BA_Gun_F(Unit ability_user) : base(ability_user) {
        DamageSources.Add(new DamageSource(200, 300, Constants.DamageType.Ranged));
        NameOfAnimationToAutoPlay = User.Actions.TryingToMoveInDirection.Count > 0 ? "Gun_F_Running" : "Gun_F";
        CanMoveWhileUsing = true;
    }

    public override void CallAbilityEvent1()
    {
        if (HoldingMainButton && ButtonPressedCounter == 1)
        {
            if (User.Ammo < GetAmmoRequiredToUseAbility(typeof(BA_Gun_S)))
            {
                UIManager.Instance.DisplayNotEnoughAmmoWarning();
                return;
            }
            User.Actions.CurrentAbilityBeingPerformed = new BA_Gun_S(User);
        }
        else if(ButtonHoldDuration > 0.2f && ButtonPressedCounter > 1)
        {
            if (User.Ammo < GetAmmoRequiredToUseAbility(typeof(BA_Gun_FS)))
            {
                UIManager.Instance.DisplayNotEnoughAmmoWarning();
                return;
            }
            User.Actions.CurrentAbilityBeingPerformed = new BA_Gun_FS(User);
        }
        else if (ButtonPressedCounter > 1)
        {
            if (User.Ammo < GetAmmoRequiredToUseAbility(typeof(BA_Gun_FF)))
            {
                UIManager.Instance.DisplayNotEnoughAmmoWarning();
                return;
            }
            User.Actions.CurrentAbilityBeingPerformed = new BA_Gun_FF(User);
            ((BA_Gun_FF)User.Actions.CurrentAbilityBeingPerformed).HoldingMainButton = HoldingMainButton;
        }
    }

    public override void AdditionalActionsOnUpdate()
    {
        base.AdditionalActionsOnUpdate();
        if(_shotBullet && Player.Instance.Actions.TryingToMoveInDirection.Count > 0 && Player.Instance.Animator.GetCurrentAnimatorClipInfo(0).Length > 0 && Player.Instance.Animator.GetCurrentAnimatorClipInfo(0)[0].clip.name == "Gun_F") {
            Player.Instance.PlayAnimation("Gun_F_Running", 0.05f / Player.Instance.RangedAttackSpeed.Current, 0.5f);
        }
        else if(_shotBullet && Player.Instance.Actions.TryingToMoveInDirection.Count == 0 && Player.Instance.Animator.GetCurrentAnimatorClipInfo(0).Length > 0 && Player.Instance.Animator.GetCurrentAnimatorClipInfo(0)[0].clip.name == "Gun_F_Running") {
            Player.Instance.PlayAnimation("Gun_F", 0.05f/ Player.Instance.RangedAttackSpeed.Current, 0.5f);
        }
    }

    public override void OnMainButtonPress()
    {
        base.OnMainButtonPress();
        if (CanFollowUpAttack)
        {
            if (User.Ammo < GetAmmoRequiredToUseAbility(typeof(BA_Gun_FF)))
            {
                UIManager.Instance.DisplayNotEnoughAmmoWarning();
                return;
            }
            User.Actions.CurrentAbilityBeingPerformed = new BA_Gun_FF(User);
            ((BA_Gun_FF)User.Actions.CurrentAbilityBeingPerformed).HoldingMainButton = HoldingMainButton;
        }
    }

    public override void AdditionalAbilitySpecificActionsOnShootingProjectile(Projectile projectile)
    {
        base.AdditionalAbilitySpecificActionsOnShootingProjectile(projectile);
        _shotBullet = true;
        Utils.PlaySoundEffect(User.AudioSource, "Gun/Gun_BasicAttack" + Utils.GetRandomSoundNumber("Gun_BasicAttack"), 0.6f);
        GameController.Instance.WaitAndRunMethod(0.5f / Player.Instance.RangedAttackSpeed.Current, CallAbilityEvent1);
    }
}