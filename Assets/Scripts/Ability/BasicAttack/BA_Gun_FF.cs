using UnityEngine;

public class BA_Gun_FF : BasicAttack {

    public static int AmmoRequiredToUseAbility = 1;
    private bool _shotBullet = false;
    public BA_Gun_FF(Unit ability_user) : base(ability_user) {
        DamageSources.Add(new DamageSource(200, 300, Constants.DamageType.Ranged));
        NameOfAnimationToAutoPlay = User.Actions.TryingToMoveInDirection.Count > 0 ? "Gun_FF_Running" : "Gun_FF";
        CanMoveWhileUsing = true;
        TransitionIntoAnimationDuration = 0;
    }

    public override void CallAbilityEvent1()
    {
        if (ButtonHoldDuration > 0.2f && ReleasedMainButton == false)
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
        CanFollowUpAttack = true;
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

    public override void AdditionalActionsOnUpdate()
    {
        base.AdditionalActionsOnUpdate();
        if(_shotBullet && Player.Instance.Actions.TryingToMoveInDirection.Count > 0 && Player.Instance.Animator.GetCurrentAnimatorClipInfo(0).Length > 0 && Player.Instance.Animator.GetCurrentAnimatorClipInfo(0)[0].clip.name == "Gun_FF") {
            Player.Instance.PlayAnimation("Gun_FF_Running", 0.05f / Player.Instance.RangedAttackSpeed.Current, 0.5f);
        }
        else if(_shotBullet && Player.Instance.Actions.TryingToMoveInDirection.Count == 0 && Player.Instance.Animator.GetCurrentAnimatorClipInfo(0).Length > 0 && Player.Instance.Animator.GetCurrentAnimatorClipInfo(0)[0].clip.name == "Gun_FF_Running") {
            Player.Instance.PlayAnimation("Gun_FF", 0.05f/ Player.Instance.RangedAttackSpeed.Current, 0.5f);
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