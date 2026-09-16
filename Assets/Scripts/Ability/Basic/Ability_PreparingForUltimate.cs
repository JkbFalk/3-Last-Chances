using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class Ability_PreparingForUltimate : Ability
{
    public Ability_PreparingForUltimate(Unit ability_user) : base(ability_user) {
        NameOfAnimationToAutoPlay =  "PreparingForUltimate_" + ((Player.Instance.CurrentStance.StanceEffect is Stance_MindOverMatter || Player.Instance.CurrentStance.StanceEffect is Stance_PowerWithoutLimit) ? "Magic" : Player.Instance.CurrentWeaponDamageType);
        CanAlwaysBeInterruptedBy.AddRange(new List<AbilityInterruptType> {AbilityInterruptType.BasicAttack, AbilityInterruptType.Block, AbilityInterruptType.Dodge, AbilityInterruptType.EnergyAbility, AbilityInterruptType.StanceSwitch});
    }

    public override void OnAbilityStart()
    {
        base.OnAbilityStart();
        Player.Instance.PreparingForUltimate = true;
        Player.Instance.AddCooldown(new Cooldown(typeof(Ability_PreparingForUltimate), Constants.PREPARING_FOR_ULTIMATE_COOLDOWN, Player.Instance));
    }

    public override void OnAbilityEnd()
    {
        base.OnAbilityEnd();
        Player.Instance.PlayAnimation(Player.Instance.InCombat ? "IdleInCombat" : "Idle");
        foreach(Stance.EquippedAbility ability in Player.Instance.CurrentStance.Abilities) {
            ability.RefreshDisplayForEquippedAbility();
        }
        foreach(Transform child in UIManager.Objects.UltimateUses.transform) {
            child.GetComponent<Image>().color = Color.white;
        } 
        Player.Instance.PreparingForUltimate = false;
        Player.Instance.CanStopPreparingForUltimate = false;
        Player.Instance.WillStopPreparingForUltimate = false;
    }

    public override void CallAbilityEvent1()
    {

    }

    public override void CallAbilityEvent2()
    {
        if(Player.Instance.PreparingForUltimate)
        {
            EndThisAbility();
        }
    }
}