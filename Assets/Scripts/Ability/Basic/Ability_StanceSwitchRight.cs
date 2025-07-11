using System.Collections.Generic;
using UnityEngine;

public class Ability_StanceSwitchRight : Ability_StanceSwitch
{

    public Ability_StanceSwitchRight(Unit ability_user) : base(ability_user) {
        int rightIndex = Player.Instance.CurrentStance == SaveFile.Instance.Stances[0] ? 1 : Player.Instance.CurrentStance == SaveFile.Instance.Stances[1] ? 2 : 3;
        NameOfAnimationToAutoPlay = SaveFile.Instance.Stances[rightIndex].WeaponClass.ToString() + "_StanceSwitch";
    }

    public override void OnAbilityStart()
    {
        Stance current_stance = Player.Instance.CurrentStance;
        List<Stance> stances = SaveFile.Instance.Stances;
        if (current_stance == stances[0])
        {
            Player.Instance.CurrentStance = stances[1];
        }
        else if (current_stance == stances[1])
        {
            Player.Instance.CurrentStance = stances[2];
        }
        else if (current_stance == stances[2])
        {
            Player.Instance.CurrentStance = stances[0];
        }
        UIManager.Instance.ShowStanceRotateRight();
        base.OnAbilityStart();
    }
}