using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ability_Heal : Ability
{
    public bool PressedHealButton = false;
    public bool ListeningForSecondInput = false;
    public bool HealedTwice = false;
    public Ability_Heal(Unit ability_user) : base(ability_user)
    {
        NameOfAnimationToAutoPlay = "Heal";
    }

    public override void OnAbilityStart() 
    {
        base.OnAbilityStart();
        Utils.CopyItemAppearanceForPlayer(Constants.ItemCategory.Tool, "Heal_" + (SaveFile.Instance.HealUpgrades > 10 ? "10" : SaveFile.Instance.HealUpgrades));
    }

    public override void CallAbilityEvent1()
    {
        ListeningForSecondInput = true;
    }

    public override void CallAbilityEvent2()
    {
        Utils.PlaySoundEffect(User.AudioSource, "Ability/Drink", 0.7f);
        SaveFile.Instance.HealChargesRemaining--;
        int extraHealAmount = 
            SaveFile.Instance.HealUpgrades >= 14 ? 3000 :
            SaveFile.Instance.HealUpgrades >= 12 ? 2300 :
            SaveFile.Instance.HealUpgrades >= 10 ? 1800 :
            SaveFile.Instance.HealUpgrades >= 8 ? 1300 :
            SaveFile.Instance.HealUpgrades >= 6 ? 900 :
            SaveFile.Instance.HealUpgrades >= 4 ? 550 :
            SaveFile.Instance.HealUpgrades >= 2 ? 250 : 0;
        Player.Instance.AddEffect(new Effect_ChangeStat(Player.Instance.Health, new(this)) {RegenerationFlatAmount = 
            (500 + extraHealAmount) / 2 }, 2);
    }

    public override void CallAbilityEvent3() {
        if(PressedHealButton && HealedTwice == false && SaveFile.Instance.HealChargesRemaining > 0) {
            HealedTwice = true;
            Player.Instance.PlayAnimation("Heal", 0.02f, 0.35f);
        }
    } 
}
