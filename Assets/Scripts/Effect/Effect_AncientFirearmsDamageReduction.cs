using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Effect_AncientFirearmsDamageReduction : Effect { 

    public Effect_ChangeStat DamageReductionBuff;

    public Effect_AncientFirearmsDamageReduction(SourceOfEffect source_of_effect) : base(source_of_effect)
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
        if (DamageReductionBuff != null && DamageReductionBuff.EffectEnded == false)
        {
            DamageReductionBuff.EndThisEffect();
        }
        int ancientCount = 1;
        if(SaveFile.Instance.EquippedHelmet is Helmet_Ancient) {
            ancientCount++;
        }
        if(SaveFile.Instance.EquippedArmor is Armor_Ancient) {
            ancientCount++;
        }
        if(SaveFile.Instance.EquippedGloves is Gloves_Ancient) {
            ancientCount++;
        }
        if(SaveFile.Instance.EquippedBoots is Boots_Ancient) {
            ancientCount++;
        }
        DamageReductionBuff = new Effect_ChangeStat(Player.Instance.DamageReduction, SourceOfEffect) {PercentageModifier = FirstParameter * ancientCount};
        Player.Instance.AddEffect(DamageReductionBuff);
    }
}
