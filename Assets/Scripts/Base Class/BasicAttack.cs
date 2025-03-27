using UnityEngine;

public abstract class BasicAttack : Ability {
    public int ComboCounter = 1;
    public bool ReleasedMainButton = false;
    public float ButtonHoldDuration = 0;
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
    public int ButtonPressedCounter = 1;
    public bool DealingDamage {
        get {
            return (Player.Instance.CurrentStance.WeaponCategory == Constants.ItemCategory.Heavy && Player.Instance.SpriteRenderers["Heavy"].Weapon.DealingDamage) ||
            (Player.Instance.CurrentStance.WeaponCategory == Constants.ItemCategory.Light && Player.Instance.SpriteRenderers["Light Right"].Weapon.DealingDamage);
        }
    }

    public BasicAttack(Unit ability_user) : base(ability_user) {
        CanAlwaysBeInterruptedBy.Add(AbilityInterruptType.StanceSwitch);
        HoldingMainButton = true;
        Properties.Add(AbilityProperty.BasicAttack);
    }

    public override void OnMainButtonPress() {
        ButtonPressedCounter++;
        HoldingMainButton = true;
        ButtonHoldDuration = 0;
    }

    public override void OnMainButtonRelease() {
        ReleasedMainButton = true;
        HoldingMainButton = false;
        ButtonHoldDuration = 0;
    }

    public override void AdditionalActionsOnUpdate()
    {
        if(HoldingMainButton)
        {
            ButtonHoldDuration += Time.deltaTime;
        }
    }

    public static BasicAttack GetPolearmStrongAttack(int combo_counter) {
        if(SaveFile.Instance.EquippedHeavyWeapon is Polearm_ChargeLance) {
            return new BA_Polearm_Charge(Player.Instance);
        }
        else if(SaveFile.Instance.EquippedHeavyWeapon is Polearm_Harpoon || SaveFile.Instance.EquippedHeavyWeapon is Polearm_Javelin) {
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