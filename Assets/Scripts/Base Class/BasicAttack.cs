using UnityEngine;

public abstract class BasicAttack : Ability {
    public int ComboCounter = 1;
    private bool _canFollowUpAttack = false;
    protected bool CanFollowUpAttack {
        get {
            return _canFollowUpAttack;
        }
        set {
            _canFollowUpAttack = value;
            CanInterruptCurrentAbility = true;
        }
    }
    public bool DealingDamage {
        get {
            return (Player.Instance.CurrentStance.WeaponType == Constants.ItemType.Heavy && Player.Instance.SpriteRenderers["Heavy"].Weapon.DealingDamage) ||
            (Player.Instance.CurrentStance.WeaponType == Constants.ItemType.Light && Player.Instance.SpriteRenderers["Light Right"].Weapon.DealingDamage);
        }
    }

    public BasicAttack(Unit ability_user) : base(ability_user) {
        CanAlwaysBeInterruptedBy.Add(AbilityInterruptType.StanceSwitch);
        Properties.Add(Property.BasicAttack);
    }

    public override void OnBasicAttackButtonPress() {
    }

    public override void OnBasicAttackButtonRelease() {
    }

    public override void AdditionalActionsOnUpdate()
    {
    }

    public static BasicAttack GetPolearmStrongAttack(int combo_counter) {
        if(Player.Instance.CheckIfUnderEffectWithGivenId("ReplaceAllBasicAttacksWithChargeAndImproveDamage")) {
            return new BA_Polearm_Charge(Player.Instance);
        }
        else if(Player.Instance.CheckIfUnderEffectWithGivenId("ReplaceAllBasicAttacksWithThrowAddPullAndIncreaseStagger") || Player.Instance.CheckIfUnderEffectWithGivenId("ReplaceAllBasicAttacksWithThrowAndIncreaseInjury")) {
            return new BA_Polearm_Throw(Player.Instance);
        }
        Unit closestTarget = Player.Instance.GetClosestValidTarget(true);
        if((Player.Instance.CurrentTarget != null && Vector2.Distance(Player.Instance.CurrentTarget.transform.position, Player.Instance.transform.position) > 6) || (Player.Instance.CurrentTarget == null && closestTarget != null && Vector2.Distance(closestTarget.transform.position, Player.Instance.transform.position) > 6)) {
            return new BA_Polearm_Throw(Player.Instance);
        }
        else if((Player.Instance.CurrentTarget != null && Vector2.Distance(Player.Instance.CurrentTarget.transform.position, Player.Instance.transform.position) > 3) || (Player.Instance.CurrentTarget == null && closestTarget != null && Vector2.Distance(closestTarget.transform.position, Player.Instance.transform.position) > 3)) {
            return new BA_Polearm_Charge(Player.Instance);
        }
        else {
            return new BA_Polearm_Sweep(Player.Instance, combo_counter);
        }
    }
}