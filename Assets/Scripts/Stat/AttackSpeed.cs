using TMPro;
using UnityEngine;

public class AttackSpeed : Stat {
    public Constants.DamageType Category = Constants.DamageType.None;

    public AttackSpeed(Constants.DamageType category, Unit stat_owner, float base_amount) : base(stat_owner, base_amount) {
        if(stat_owner is Player) {
            MenuStatDisplay = CanvasElements.MenuCanvas.StatList.transform.Find(category.ToString() + "AttackSpeed/Value").GetComponent<TextMeshProUGUI>();
        }
        Category = category;
        MaximumValue = Category == Constants.DamageType.Heavy ? 1.5f : Category == Constants.DamageType.Light ? 2.5f : 2.0f;
        Owner = stat_owner;
        Base = base_amount;
        Maximum = base_amount;
        Current = base_amount;
    }

    public override void AdditionalStatSpecificActionsAfterCurrentValueChanged() {
        if(Category != Constants.DamageType.None)
        {
            Owner.Animator.SetFloat(Category.ToString() + " Attack Speed", ScaledWithCombatSpeed * ((Owner is Player && Player.Instance.HitStopFramesRemaining > 0) ? 0.1f : 1));
        }
        if(ShouldInvoke && Current < 0.2f) {
            ChangeCurrentValueWithoutInvoking(0.2f);
        }
    }

    public float ScaledWithCombatSpeed
    {
        get
        {
            if(DebugController.MaxAttackSpeed) {
                return 3;
            }
            return Current;
        }
    }

    public override string ToString()
    {
        return Category == Constants.DamageType.Heavy ? "HeavyAttackSpeed" : Category == Constants.DamageType.Light ? "LightAttackSpeed"  : Category == Constants.DamageType.Ranged ? "RangedAttackSpeed"  : Category == Constants.DamageType.Magic ? "MagicAttackSpeed" : "AttackSpeed";
    }

    public override void UpdateMenuStatDisplayValue()
    {
        MenuStatDisplay.text = Utils.GetFormattedFloat(Current, 1, true) + " (" + Utils.GetFormattedFloat(Base, 1, true) + ")";
    }
}