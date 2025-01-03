using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ability_ExpertBadge : Ability
{

    public Ability_ExpertBadge(Unit ability_user, Item item) : base(ability_user)
    {
        ItemBeingUsed = item;
        ItemBeingUsed.ItemUseAbility = this;
        NameOfAnimationToAutoPlay = "ActivateItem";
    }
    public override void OnAbilityStart()
    {
        base.OnAbilityStart();
        Utils.CopyItemAppearanceForPlayer(Constants.ItemCategory.Tool, ItemBeingUsed.GetType().ToString() + "_" + ItemBeingUsed.Grade.ToString());
    }

    public override void CallAbilityEvent1()
    {
        User.Actions.ActivateConsumableUsage();
        Tool_ExpertBadge badge = (Tool_ExpertBadge)ItemBeingUsed;
        Utils.PlaySoundEffect(Player.Instance.AudioSource, "Item/ExpertBadge_Use", 0.25f);
        Utils.CreateVisualEffect(new(ItemBeingUsed), "EnergyGain");
        User.AddEffect(new Effect_ChangeStat(User.Energy, new(this)) {RegenerationFlatAmount = badge.TotalEnergyRestored[badge.GradeIndex] / badge.Duration[badge.GradeIndex]}, badge.Duration[badge.GradeIndex]);
        User.AddEffect(new Effect_AddExtraCooldown(typeof(Tool_ExpertBadge), badge.CooldownIncrease[badge.GradeIndex], new(this)));
    }
}
