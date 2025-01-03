using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ability_FireStarter : Ability
{
    Projectile projectile;

    public Ability_FireStarter(Unit ability_user, Item item) : base(ability_user)
    {
        if (item.DamageSources != null && item.DamageSources.Count > 0)
        {
            DamageSources = item.DamageSources;
        }
        ItemBeingUsed = item;
        ItemBeingUsed.ItemUseAbility = this;
        NameOfAnimationToAutoPlay = "InvokeItem";
        DamageTriggerLimit = DamageTriggerLimitType.OncePerUnitFromEachSource;
    }

    public override void OnAbilityStart()
    {
        base.OnAbilityStart();
        Utils.CopyItemAppearanceForPlayer(Constants.ItemCategory.Tool, ItemBeingUsed.GetType().ToString() + "_" + ItemBeingUsed.Grade.ToString());
    }

    public override void CallAbilityEvent1()
    {
        Utils.PlaySoundEffect(Player.Instance.AudioSource, "Item/FireStarter_Use", 0.6f);
        projectile = Utils.CreateProjectile(new(this), ItemBeingUsed.GetType().ToString().Replace("Permanent_", "") + "_" + ItemBeingUsed.Grade.ToString());
        projectile.transform.up = User.Actions.IsFlipped ? Vector2.left : Vector2.right;
        ItemBeingUsed.ToolObject = projectile.gameObject;
    }

    public override void HandleEnemyHit(Unit unit_getting_attacked, DamagingObject object_hitting, Collider2D collider_being_hit)
    {
        if(object_hitting is Projectile) {
            ((Projectile)object_hitting).FlightSpeed = 0;
            ((Projectile)object_hitting).MakeObjectDisappear(0);
        }
        if(object_hitting.name == "FireStarter_" + ItemBeingUsed.Grade.ToString())
        {
            Utils.PlaySoundEffect(unit_getting_attacked.AudioSource, "Item/FireStarter_Explosion", 0.65f);
        }
        ItemBeingUsed.OnEnemyHit(unit_getting_attacked, object_hitting, collider_being_hit);
        base.HandleEnemyHit(unit_getting_attacked, object_hitting, collider_being_hit);
    }

    public override void ExtraBehaviourOnHit(Damage damage)
    {
        damage.TargetOfDamage.AddEffect(new Effect_KnockedBack(damage, new(this)));
        damage.TargetOfDamage.AddEffect(new Effect_Burn(3 * ItemBeingUsed.GetMultiplierForGrade(), new(this)));
    }
}
