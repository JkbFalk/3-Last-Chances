using System.Collections.Generic;
using UnityEngine;

public class Ability_StanceSwitchRight : Ability_StanceSwitch
{

    public Ability_StanceSwitchRight(Unit ability_user) : base(ability_user) {

    }

    public override void OnAbilityStart()
    {
        base.OnAbilityStart();
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
    }
}