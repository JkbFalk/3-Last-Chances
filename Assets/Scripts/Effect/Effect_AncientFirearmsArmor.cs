using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Effect_AncientFirearmsArmor : Effect { 

    public Effect_ChangeStat ArmorBuff;

    public Effect_AncientFirearmsArmor(SourceOfEffect source_of_effect) : base(source_of_effect)
    {
        Type = EffectType.Buff;
        Listeners.Add(EventManager.ItemEquipped);
    }


    public override void OnStart() {
        base.OnStart();
        Activate();
    }

    public override void OnInvokeItemEquipped(Item item1, Item item2)
    {
        base.OnInvokeItemEquipped(item1, item2);
        Activate();
    }

    public override void OnEnd()
    {
        base.OnEnd();
        Activate();
    }

    public void Activate() {
        if (ArmorBuff != null && ArmorBuff.EffectEnded == false)
        {
            ArmorBuff.EndThisEffect();
        }
        int ancientCount = 1;
        if(SaveFile.Instance.EquippedHelmet is Helmet_Ancient) {
            ancientCount++;
        }
        if(SaveFile.Instance.EquippedOutfit is Outfit_Ancient) {
            ancientCount++;
        }
        if(SaveFile.Instance.EquippedGloves is Gloves_Ancient) {
            ancientCount++;
        }
        if(SaveFile.Instance.EquippedBoots is Boots_Ancient) {
            ancientCount++;
        }
        ArmorBuff = new Effect_ChangeStat(Player.Instance.Armor, SourceOfEffect) {PercentageAmount = FirstParameter * ancientCount};
        Player.Instance.AddEffect(ArmorBuff);
    }
}
