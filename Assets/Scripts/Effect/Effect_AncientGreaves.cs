using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Effect_AncientGreaves : Effect { 

    public Effect_ChangeCompositeStat AttackSpeedBuff;

    public Effect_AncientGreaves(SourceOfEffect source_of_effect) : base(source_of_effect)
    {
        Type = EffectType.Buff;
        Listeners.Add(EventManager.ItemEquipped);
    }


    public override void OnStart() {
        base.OnStart();
        if(SaveFile.Instance.EquippedArmor is Armor_Ancient && SaveFile.Instance.EquippedBoots is Boots_Ancient) {
            AddBuffs();
        }
    }

    public override void OnInvokeItemEquipped(Item item1, Item item2)
    {
        if (AttackSpeedBuff != null && (item1 is Armor_Ancient || item1 is Boots_Ancient))
        {
            AttackSpeedBuff.EndThisEffect();
        }
        else if ((item2 is Boots_Ancient && SaveFile.Instance.EquippedArmor is Armor_Ancient) || (item2 is Armor_Ancient && SaveFile.Instance.EquippedBoots is Boots_Ancient))
        {
            AddBuffs();
        }
    }

    public void AddBuffs() {
        AttackSpeedBuff = new Effect_ChangeCompositeStat(Player.Instance, Effect_ChangeCompositeStat.CompositeStat.AttackSpeed, SourceOfEffect) {PercentageModifier = FirstParameter};
        Player.Instance.AddEffect(AttackSpeedBuff);
    }
}
