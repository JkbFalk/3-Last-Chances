using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.InputSystem.Controls;

public class Ability_ThrowItem : Ability
{
    Projectile projectile;
    public Ability_ThrowItem(Unit ability_user, Item item) : base(ability_user)
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
        Utils.PlaySoundEffect(Player.Instance.AudioSource, "Item/VacuumGrenade_Use", 0.9f);
        Utils.CopyItemAppearanceForPlayer(Constants.ItemType.Tool, ItemBeingUsed.GetType().ToString() + "_" + ItemBeingUsed.Grade.ToString());
    }

    public override void CallAbilityEvent1()
    {
        projectile = Utils.CreateProjectile(new(this), ItemBeingUsed.GetType().ToString().Replace("Tool_", ""));
        projectile.transform.up = User.Actions.IsFlipped ? Vector2.left : Vector2.right;
        ItemBeingUsed.ToolObject = projectile.gameObject;
    }

    public override void HandleEnemyHit(Unit unit_getting_attacked, DamagingObject object_hitting, Collider2D collider_being_hit)
    {
        ItemBeingUsed.OnEnemyHit(unit_getting_attacked, object_hitting, collider_being_hit);
        base.HandleEnemyHit(unit_getting_attacked, object_hitting, collider_being_hit);
    }
}