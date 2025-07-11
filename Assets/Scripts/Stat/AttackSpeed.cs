using TMPro;
using UnityEngine;

public class AttackSpeed : Stat {
    public Constants.DamageType Type = Constants.DamageType.None;

    public AttackSpeed(Constants.DamageType type, Unit stat_owner, float base_amount) : base(stat_owner, base_amount) {
        if(stat_owner is Player) {
            MenuStatDisplay = MenuManager.Objects.CharacterStatList.transform.Find(type.ToString() + "AttackSpeed/Value").GetComponent<TextMeshProUGUI>();
        }
        Type = type;
        MaximumValue = Type == Constants.DamageType.Heavy ? 1.5f : Type == Constants.DamageType.Light ? 2.5f : 2.0f;
        Owner = stat_owner;
        Base = base_amount;
        Maximum = base_amount;
        Current = base_amount;
    }

    public override void AdditionalStatSpecificActionsAfterCurrentValueChanged() {
        if(Type != Constants.DamageType.None)
        {
            Owner.Animator.SetFloat(Type.ToString() + " Attack Speed", ScaledWithCombatSpeed * ((Owner is Player && Player.Instance.HitStopFramesRemaining > 0) ? 0.1f : 1));
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
        return Type == Constants.DamageType.Heavy ? "HeavyAttackSpeed" : Type == Constants.DamageType.Light ? "LightAttackSpeed"  : Type == Constants.DamageType.Ranged ? "RangedAttackSpeed"  : Type == Constants.DamageType.Magic ? "MagicAttackSpeed" : "AttackSpeed";
    }

    public override void UpdateMenuStatDisplayValue()
    {
        MenuStatDisplay.text = Utils.GetFormattedFloat(Current, 2) + " (" + Utils.GetFormattedFloat(Base, 2) + ")";
    }
}