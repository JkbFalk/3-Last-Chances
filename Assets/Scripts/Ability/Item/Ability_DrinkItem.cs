using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Ability_DrinkItem : Ability
{
    public Ability_DrinkItem(Unit ability_user, Item item) : base(ability_user)
    {
        ItemBeingUsed = item;
        ItemBeingUsed.ItemUseAbility = this;
        NameOfAnimationToAutoPlay = String.IsNullOrEmpty(ItemBeingUsed.CustomAnimation) ? "DrinkItem_Fast" : ItemBeingUsed.CustomAnimation;
    }

    public override void OnAbilityStart()
    {
        base.OnAbilityStart();
        Utils.CopyItemAppearanceForPlayer(Constants.ItemType.Tool, ItemBeingUsed.GetType().ToString() + "_" + ItemBeingUsed.Grade.ToString());
    }
}