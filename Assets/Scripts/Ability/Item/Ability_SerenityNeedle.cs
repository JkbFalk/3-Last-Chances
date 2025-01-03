using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.InputSystem.Controls;

public class Ability_SerenityNeedle : Ability
{
    public static int[] SleepDurationBasedOnGrade = new int[] {6, 8, 12, 18, 30};
    Projectile projectile;
    public Ability_SerenityNeedle(Unit ability_user, Item item) : base(ability_user)
    {
        if(item.DamageSources != null && item.DamageSources.Count > 0)
        {
            DamageSources = item.DamageSources;
        }
        ItemBeingUsed = item;
        ItemBeingUsed.ItemUseAbility = this;
    }

    public override void OnAbilityStart()
    {
        base.OnAbilityStart();
        Utils.CopyItemAppearanceForPlayer(Constants.ItemCategory.Tool, "SerenityNeedle");
    }

    public override void HandleEnemyHit(Unit unit_getting_attacked, DamagingObject object_hitting, Collider2D collider_being_hit)
    {
        base.HandleEnemyHit(unit_getting_attacked, object_hitting, collider_being_hit);
        unit_getting_attacked.AddEffect(new Effect_Sleep(new(this)), SleepDurationBasedOnGrade[Item.GetIndexForItemGrade(ItemBeingUsed.Grade)]);
    }
}