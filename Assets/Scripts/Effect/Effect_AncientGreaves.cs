using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Effect_AncientGreaves : Effect { 

    public Effect_ChangeCompositeStat SpeedBuff;
    public float BuffAmount = 0;

    public Effect_AncientGreaves(SourceOfEffect source_of_effect) : base(source_of_effect)
    {
        Type = EffectType.Buff;
        Listeners.Add(EventManager.ItemEquipped);
    }

    public override void OnEffectValueChanged()
    {
        BuffAmount = 0.125f * NonLinearEffectValue;
        DescriptionParameters = new List<string> { Utils.GetFormattedFloat(BuffAmount) };
    }

    public override void OnStart() {
        base.OnStart();
        if(SaveFile.Instance.EquippedArmor is Armor_Ancient && SaveFile.Instance.EquippedBoots is Boots_Ancient) {
            AddBuffs();
        }
    }

    public override void OnInvokeItemEquipped(Item item1, Item item2)
    {
        if (SpeedBuff != null && (item1 is Armor_Ancient || item1 is Boots_Ancient))
        {
            SpeedBuff.EndThisEffect();
        }
        else if ((item2 is Boots_Ancient && SaveFile.Instance.EquippedArmor is Armor_Ancient) || (item2 is Armor_Ancient && SaveFile.Instance.EquippedBoots is Boots_Ancient))
        {
            AddBuffs();
        }
    }

    public void AddBuffs() {
        SpeedBuff = new Effect_ChangeCompositeStat(Player.Instance, Effect_ChangeCompositeStat.CompositeStat.AttackSpeed, SourceOfEffect) {PercentageAmount = BuffAmount};
        Player.Instance.AddEffect(SpeedBuff);
    }
}
