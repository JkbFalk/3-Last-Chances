using System.Collections.Generic;
using UnityEngine;

public class Ability_StanceSwitch : Ability {

    public Ability_StanceSwitch(Unit ability_user) : base(ability_user)
    {
        NameOfAnimationToAutoPlay = Player.Instance.CurrentStance.WeaponClass.ToString() + "_StanceSwitch";
        CanAlwaysBeInterruptedBy.AddRange(new List<AbilityInterruptType> {AbilityInterruptType.BasicAttack, AbilityInterruptType.Block, AbilityInterruptType.Dodge, AbilityInterruptType.EnergyAbility, AbilityInterruptType.StanceSwitch});
        TransitionIntoAnimationDuration = 0.2f;
        TransitionOutOfAnimationDuration = 0.2f;
        AddCustomSound("Gun1", "Gun/Take out gun", 0.1f);
        AddCustomSound("Gun2", "Gun/Revolver Hammer cock 2", 0.05f);
        AddCustomSound("Bow", "Bow/Take out bow", 0.5f);
        AddCustomSound("Polearm", "Polearm/Polearm_Swing9", 0.4f);
        AddCustomSound("Greatsword", "Greatsword/Greatsword_Swing9", 0.4f);
        AddCustomSound("TwinBlades", "TwinBlades/TwinBlades_Swing20", 0.4f);
        AddCustomSound("Daggers", "Daggers/Daggers_Swing1", 0.4f);
        AddCustomSound("Magic", "Magic/Magic Take Out", 0.4f);
    }

    public override void OnAbilityStart()
    {
        User.SetDefaultSortingOrder();
        base.OnAbilityStart();
    }
}